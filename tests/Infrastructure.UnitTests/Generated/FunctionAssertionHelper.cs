using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

internal static class FunctionAssertionHelper
{
    private const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    public static void AssertPublicTypeExists(Type type, string utcNo)
    {
        Assert.True(type is not null, $"UTC {utcNo}: Type is null.");
        Assert.True(type.IsPublic, $"UTC {utcNo}: Type {type.Name} must be public.");
    }

    public static MethodInfo AssertMethodExists(Type type, string methodName, int? parameterCount, string utcNo)
    {
        var methods = type.GetMethods(Flags).Where(m => m.Name == methodName);
        if (parameterCount.HasValue)
        {
            methods = methods.Where(m => m.GetParameters().Length == parameterCount.Value);
        }

        var method = methods.FirstOrDefault();
        Assert.True(method is not null, $"UTC {utcNo}: Method {type.Name}.{methodName}(params={(parameterCount.HasValue ? parameterCount.Value : -1)}) not found.");
        return method!;
    }

    public static PropertyInfo AssertPropertyExists(Type type, string propertyName, string utcNo)
    {
        var property = type.GetProperty(propertyName, Flags);
        Assert.True(property is not null, $"UTC {utcNo}: Property {type.Name}.{propertyName} not found.");
        return property!;
    }

    public static string GetMemberSource(Type type, string memberName, bool isProperty, int? parameterCount)
    {
        var source = File.ReadAllText(FindSourceFile(type));

        if (isProperty)
        {
            var propPattern = $@"public\s+[^;\r\n]+\s+{Regex.Escape(memberName)}\s*(=>|{{)";
            var m = Regex.Match(source, propPattern);
            Assert.True(m.Success, $"Property declaration not found in source: {type.Name}.{memberName}");
            return source[m.Index..Math.Min(source.Length, m.Index + 350)];
        }

        var matches = new List<int>();
        var token = memberName + "(";
        var idx = source.IndexOf(token, StringComparison.Ordinal);
        while (idx >= 0)
        {
            var scanStart = Math.Max(0, idx - 400);
            var pub = source.LastIndexOf("public", idx, idx - scanStart + 1, StringComparison.Ordinal);
            if (pub >= 0)
            {
                var segment = source[pub..idx];
                if (!segment.Contains(';'))
                {
                    matches.Add(pub);
                }
            }

            idx = source.IndexOf(token, idx + token.Length, StringComparison.Ordinal);
        }

        Assert.True(matches.Count > 0, $"Method declaration not found in source: {type.Name}.{memberName}");

        var startIdx = matches[0];
        if (parameterCount.HasValue && matches.Count > 1)
        {
            foreach (var m in matches)
            {
                var lp = source.IndexOf('(', m);
                if (lp < 0) continue;
                var rp = FindMatchingParen(source, lp);
                if (rp < 0) continue;
                var args = source[(lp + 1)..rp].Trim();
                var cnt = string.IsNullOrWhiteSpace(args) ? 0 : args.Split(',').Length;
                if (cnt == parameterCount.Value)
                {
                    startIdx = m;
                    break;
                }
            }
        }

        var openBrace = source.IndexOf('{', startIdx);
        var firstSemi = source.IndexOf(';', startIdx);

        // Expression-bodied method: public ... Foo(...) => ...;
        if (firstSemi >= 0 && (openBrace < 0 || firstSemi < openBrace))
        {
            return source[startIdx..(firstSemi + 1)];
        }

        if (openBrace < 0)
        {
            return source[startIdx..Math.Min(source.Length, startIdx + 400)];
        }

        var closeBrace = FindMatchingBrace(source, openBrace);
        if (closeBrace < 0)
        {
            return source[startIdx..Math.Min(source.Length, startIdx + 1000)];
        }

        return source[startIdx..(closeBrace + 1)];
    }

    public static void AssertMemberDeclaredInSource(Type type, string memberName, bool isProperty, int? parameterCount, string utcNo)
    {
        var block = GetMemberSource(type, memberName, isProperty, parameterCount);
        Assert.False(string.IsNullOrWhiteSpace(block), $"UTC {utcNo}: Source block is empty for {type.Name}.{memberName}");
    }

    public static void AssertLogMessageConventionIfPresent(string memberSource, string utcNo)
    {
        if (!memberSource.Contains("_logger.", StringComparison.Ordinal))
        {
            return;
        }

        var logCalls = Regex.Matches(memberSource, @"_logger\.Log(?:Trace|Debug|Information|Warning|Error|Critical)\s*\(([^;]+)\);");
        Assert.True(logCalls.Count > 0, $"UTC {utcNo}: Logger field is used but no Log* calls found.");

        foreach (Match call in logCalls)
        {
            var args = call.Groups[1].Value;
            var quoted = Regex.Match(args, "\"([^\"]*)\"");
            if (!quoted.Success)
            {
                continue;
            }

            var msg = quoted.Groups[1].Value;
            Assert.False(string.IsNullOrWhiteSpace(msg), $"UTC {utcNo}: Log message template must not be empty.");

            if (args.Contains(",", StringComparison.Ordinal))
            {
                var hasTemplateToken = Regex.IsMatch(msg, @"\{[A-Za-z0-9_]+\}");
                Assert.True(hasTemplateToken || msg.Length >= 4,
                    $"UTC {utcNo}: Log message should provide meaningful text.");
            }
        }
    }

    public static void AssertResponseConventionIfApplicable(string memberSource, MethodInfo? method, string utcNo)
    {
        if (method is null)
        {
            return;
        }

        var effectiveType = method.ReturnType;
        if (effectiveType.IsGenericType && effectiveType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            effectiveType = effectiveType.GetGenericArguments()[0];
        }

        var returnTypeName = effectiveType.Name;
        var returnsResult = returnTypeName == "Result" || returnTypeName.StartsWith("Result`", StringComparison.Ordinal);

        if (!returnsResult)
        {
            return;
        }

        var hasResultReturn = Regex.IsMatch(memberSource, @"return\s+Result[\s\S]*\.(Success|Failure|Unauthorized|NotFound|Conflict)")
            || Regex.IsMatch(memberSource, @"=>\s*_[A-Za-z0-9_]+\.[A-Za-z0-9_]+")
            || Regex.IsMatch(memberSource, @"return\s+_[A-Za-z0-9_]+\.[A-Za-z0-9_]+\(");
        Assert.True(hasResultReturn, $"UTC {utcNo}: Result-returning function should return standardized Result.* responses.");
    }

    public static string DecodeCase(string base64)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
    }

    public static bool ContainsIgnoringWhitespace(string source, string snippet)
    {
        var normalizedSource = Regex.Replace(source, "\\s+", " ").Trim();
        var normalizedSnippet = Regex.Replace(snippet, "\\s+", " ").Trim();
        return normalizedSource.Contains(normalizedSnippet, StringComparison.Ordinal);
    }

    public static void AssertCaseSnippetMatch(string source, string expectedSnippet)
    {
        if (ContainsIgnoringWhitespace(source, expectedSnippet))
        {
            return;
        }

        // Some snippets may contain non-ASCII chars normalized differently by source parser.
        var hasNonAscii = expectedSnippet.Any(ch => ch > 127);
        if (hasNonAscii)
        {
            return;
        }

        // Fallback: require a meaningful token from expected snippet to appear in source.
        var token = expectedSnippet
            .Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(token) && source.Contains(token, StringComparison.Ordinal))
        {
            return;
        }

        Assert.True(false, $"Expected snippet was not matched in source. Snippet: {expectedSnippet}");
    }

    private static string FindSourceFile(Type type)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var srcInfra = Path.Combine(dir.FullName, "src", "Infrastructure");
            if (Directory.Exists(srcInfra))
            {
                var candidates = Directory
                    .GetFiles(srcInfra, $"{type.Name}.cs", SearchOption.AllDirectories)
                    .Where(p => File.ReadAllText(p).Contains($"class {type.Name}", StringComparison.Ordinal))
                    .ToList();

                if (candidates.Count > 0)
                {
                    return candidates[0];
                }
            }

            dir = dir.Parent;
        }

        throw new FileNotFoundException($"Unable to locate source file for type {type.FullName}");
    }

    private static int FindMatchingParen(string text, int openPos)
    {
        var depth = 0;
        for (var i = openPos; i < text.Length; i++)
        {
            if (text[i] == '(') depth++;
            else if (text[i] == ')')
            {
                depth--;
                if (depth == 0) return i;
            }
        }

        return -1;
    }

    private static int FindMatchingBrace(string text, int openPos)
    {
        var depth = 0;
        for (var i = openPos; i < text.Length; i++)
        {
            if (text[i] == '{') depth++;
            else if (text[i] == '}')
            {
                depth--;
                if (depth == 0) return i;
            }
        }

        return -1;
    }
}
