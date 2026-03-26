using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class SeedContractTemplates : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Ophthalmologist Contract Template
        var ophthalmologistId = Guid.NewGuid();
        var ophthalmologistTemplate = GetOphthalmologistContractTemplate();

        // Escape single quotes for SQL
        ophthalmologistTemplate = ophthalmologistTemplate.Replace("'", "''");

        migrationBuilder.Sql($@"
            INSERT INTO ""ContractTemplates"" 
            (""Id"", ""Title"", ""Type"", ""ContractVersion"", ""ContentTemplate"", ""IsActive"", ""EffectiveDate"", ""CreatedAt"", ""IsDeleted"")
            VALUES 
            ('{ophthalmologistId}', N'Hợp đồng Bác sĩ Nhãn khoa', 1, '1.0', N'{ophthalmologistTemplate}', true, NOW(), NOW(), false)
        ");

        // Medical Organization Contract Template
        var organizationId = Guid.NewGuid();
        var organizationTemplate = GetMedicalOrganizationContractTemplate();

        // Escape single quotes for SQL
        organizationTemplate = organizationTemplate.Replace("'", "''");

        migrationBuilder.Sql($@"
            INSERT INTO ""ContractTemplates"" 
            (""Id"", ""Title"", ""Type"", ""ContractVersion"", ""ContentTemplate"", ""IsActive"", ""EffectiveDate"", ""CreatedAt"", ""IsDeleted"")
            VALUES 
            ('{organizationId}', N'Hợp đồng Tổ chức Y tế', 2, '1.0', N'{organizationTemplate}', true, NOW(), NOW(), false)
        ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            DELETE FROM ""ContractTemplates"" 
            WHERE ""Type"" IN (1, 2)
        ");
    }

    private static string GetOphthalmologistContractTemplate()
    {
        return @"<!doctype html>
<html lang=""vi"">
  <head>
    <meta charset=""UTF-8"" />
    <title>Hợp đồng Bác sĩ - Final Legal</title>
    <style>
      body {
        font-family: ""Times New Roman"", serif;
        background: #f0f0f0;
        padding: 40px;
      }
      .contract-paper {
        background: white;
        width: 210mm;
        min-height: 297mm;
        margin: 0 auto;
        padding: 20mm 25mm;
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        font-size: 13pt;
        line-height: 1.5;
        text-align: justify;
      }
      .center-bold {
        text-align: center;
        font-weight: bold;
        text-transform: uppercase;
        margin: 0;
      }
      .tieu-ngu {
        text-align: center;
        font-weight: bold;
        margin-top: 5px;
        margin-bottom: 20px;
      }
      .tieu-ngu span {
        border-bottom: 1px solid black;
        padding-bottom: 3px;
      }
      .contract-name {
        text-align: center;
        font-size: 16pt;
        font-weight: bold;
        margin-top: 30px;
        margin-bottom: 5px;
        text-transform: uppercase;
      }
      .italic {
        font-style: italic;
      }
      .bold {
        font-weight: bold;
      }
      .section-title {
        font-weight: bold;
        margin-top: 15px;
        margin-bottom: 5px;
        text-transform: uppercase;
      }
      .info-row {
        margin-bottom: 5px;
        padding-left: 20px;
      }
      ul,
      ol {
        margin-top: 5px;
        margin-bottom: 5px;
        padding-left: 30px;
      }
      li {
        margin-bottom: 5px;
      }
    </style>
  </head>
  <body>
    <div class=""contract-paper"">
      <div class=""center-bold"">CỘNG HOÀ XÃ HỘI CHỦ NGHĨA VIỆT NAM</div>
      <div class=""tieu-ngu""><span>Độc lập - Tự do - Hạnh phúc</span></div>

      <div class=""contract-name"">HỢP ĐỒNG HỢP TÁC CHUYÊN MÔN Y KHOA</div>
      <div style=""text-align: center; font-style: italic; margin-bottom: 20px"">
        Số: {{contract_code}}/HĐCM-AURA
      </div>

      <div class=""italic"">
        - Căn cứ Bộ luật Dân sự số 91/2015/QH13 ngày 24/11/2015;<br />
        - Căn cứ Luật Khám bệnh, chữa bệnh số 15/2023/QH15 (Quy định về Khám
        chữa bệnh từ xa);<br />
        - Căn cứ Nghị định 13/2023/NĐ-CP về bảo vệ dữ liệu cá nhân;<br />
        - Căn cứ nhu cầu và khả năng thực tế của các bên.
      </div>

      <p>
        Hôm nay, ngày {{day}} tháng {{month}} năm {{year}}, tại hệ thống AURA,
        chúng tôi gồm có:
      </p>

      <div style=""margin-bottom: 15px"">
        <div class=""bold"">BÊN A: CÔNG TY CỔ PHẦN CÔNG NGHỆ AURA</div>
        <div class=""info-row"">
          Địa chỉ trụ sở: Khu Công nghệ cao, Thành phố Thủ Đức, TP.HCM.
        </div>
        <div class=""info-row"">Mã số doanh nghiệp: 031XXXXXXX</div>
        <div class=""info-row"">
          Đại diện bởi Ông: <span class=""bold"">Nguyễn Văn Admin</span>
        </div>
        <div class=""info-row"">Chức vụ: Giám đốc Điều hành</div>
        <div class=""info-row"">Email: expert@aura.health</div>
      </div>

      <div style=""margin-bottom: 15px"">
        <div class=""bold"">BÊN B: BÁC SĨ {{doctor_name}}</div>
        <div class=""info-row"">
          Ngày sinh: {{dob}} &nbsp;&nbsp;|&nbsp;&nbsp; Quốc tịch: Việt Nam
        </div>
        <div class=""info-row"">
          CCCD số:
          <span class=""bold"">{{citizen_id}}</span> &nbsp;&nbsp;|&nbsp;&nbsp;
          Ngày cấp: {{issue_date}}
        </div>
        <div class=""info-row bold"">
          Chứng chỉ hành nghề khám chữa bệnh số: {{license_number}}
        </div>
        <div class=""info-row"">
          Phạm vi hoạt động: Nhãn khoa / Chẩn đoán hình ảnh
        </div>
        <div class=""info-row"">Địa chỉ thường trú: {{permanent_address}}</div>
        <div class=""info-row"">
          Điện thoại: {{phone}} &nbsp;&nbsp;|&nbsp;&nbsp; Email: {{email}}
        </div>
        <div class=""info-row"">
          Số tài khoản: {{bank_account}} tại Ngân hàng {{bank_name}}
        </div>
      </div>

      <p>
        Hai Bên cùng thỏa thuận và thống nhất ký kết Hợp đồng này với các điều
        khoản chi tiết sau đây:
      </p>

      <div class=""section-title"">ĐIỀU 1. MỤC ĐÍCH VÀ NỘI DUNG HỢP TÁC</div>
      <p>
        Bên A cung cấp nền tảng công nghệ AURA và Bên B cung cấp năng lực chuyên
        môn để thực hiện quy trình khám chữa bệnh từ xa (Telemedicine). Bên B
        cam kết thực hiện đầy đủ các bước trong quy trình chẩn đoán như sau:
      </p>
      <ol>
        <li>
          <strong>Bước 1 - Tiếp nhận và Phân loại:</strong> Tiếp nhận hồ sơ bệnh
          án và hình ảnh đáy mắt (Fundus Image) từ hệ thống. Xem xét kết quả
          phân tích gợi ý từ AI (Triage) để đánh giá mức độ rủi ro.
        </li>
        <li>
          <strong>Bước 2 - Tư vấn chuyên sâu:</strong> Thực hiện phiên tư vấn
          trực tuyến (Real-time Consultation) với bệnh nhân thông qua tính năng
          Chat hoặc Video Call trên ứng dụng để khai thác tiền sử bệnh và giải
          thích tình trạng.
        </li>
        <li>
          <strong>Bước 3 - Kết luận:</strong> Đưa ra kết luận chẩn đoán cuối
          cùng (Final Diagnosis), xây dựng lộ trình chăm sóc sức khỏe và ký xác
          nhận điện tử trên phiếu kết quả.
        </li>
      </ol>

      <div class=""section-title"">ĐIỀU 2. QUYỀN LỢI CỦA BÊN B</div>
      <p>
        <strong>2.1. Quyền lợi tài chính:</strong> Bên B được hưởng thu nhập
        theo tỷ lệ phân chia quy định tại Điều 3 của Hợp đồng này.
      </p>

      <p><strong>2.2. Quyền lợi gia tăng (Không bắt buộc):</strong></p>
      <p>
        Để nâng cao năng lực chuyên môn và uy tín cá nhân, Bên B được quyền đăng
        ký tham gia vào
        <strong>Mạng lưới Chuyên gia AURA (AURA Professional Network)</strong>.
        Khi tham gia, Bên B được quyền:
      </p>
      <ul>
        <li>
          Truy cập vào kho dữ liệu các ca bệnh điển hình (Case Study) để phục vụ
          mục đích nghiên cứu.
        </li>
        <li>
          Tham gia các buổi hội chẩn trực tuyến, trao đổi kinh nghiệm với các
          chuyên gia đầu ngành trong mạng lưới.
        </li>
      </ul>

      <div class=""section-title"">ĐIỀU 3. CƠ CHẾ TÀI CHÍNH</div>
      <p>
        3.1. Lợi nhuận được chia tương ứng với công sức đóng góp trên từng ca
        bệnh thành công:
      </p>
      <ul>
        <li>
          Bên B hưởng: <strong>{{commission_rate}}%</strong> (trên phí tư vấn
          sau thuế).
        </li>
        <li>Bên A hưởng: Phần còn lại (Phí công nghệ và vận hành nền tảng).</li>
      </ul>
      <p>
        3.2. Thời điểm thanh toán: Hệ thống tự động đối soát vào ngày 15 và 30
        hàng tháng. Tiền thù lao sẽ được chuyển khoản trong vòng 03 ngày làm
        việc sau kỳ đối soát.
      </p>

      <div class=""section-title"">ĐIỀU 4. CAM KẾT BẢO MẬT (NGHỊ ĐỊNH 13)</div>
      <p>
        Bên B cam kết tuân thủ nghiêm ngặt
        <strong>Nghị định 13/2023/NĐ-CP</strong> về bảo vệ dữ liệu cá nhân. Bên
        B tuyệt đối không sao chép, lưu trữ, phát tán hình ảnh hoặc thông tin
        bệnh nhân ra ngoài hệ thống AURA dưới bất kỳ hình thức nào. Mọi vi phạm
        sẽ bị xử lý theo quy định của pháp luật.
      </p>

      <div class=""section-title"">ĐIỀU 5. HIỆU LỰC HỢP ĐỒNG</div>
      <p>
        Hợp đồng này được lập dưới dạng dữ liệu điện tử, có giá trị pháp lý như
        bản gốc và có hiệu lực 01 (một) năm kể từ ngày ký.
      </p>

      <table style=""width: 100%; margin-top: 50px"">
        <tr>
          <td style=""text-align: center; width: 50%; vertical-align: top"">
            <div class=""bold"">ĐẠI DIỆN BÊN A</div>
            <div class=""italic"">(Ký số hệ thống)</div>
            <br /><br />
            <div
              style=""
                border: 2px solid #0056b3;
                color: #0056b3;
                display: inline-block;
                padding: 10px;
                font-weight: bold;
              ""
            >
              AURA VERIFIED
            </div>
          </td>
          <td style=""text-align: center; width: 50%; vertical-align: top"">
            <div class=""bold"">ĐẠI DIỆN BÊN B</div>
            <div class=""italic"">(Ký số)</div>
            <br /><br />
            <img src=""{{signature_url}}"" alt=""Chữ ký"" style=""height: 70px"" />
            <div class=""bold"">{{doctor_name}}</div>
          </td>
        </tr>
      </table>
    </div>
  </body>
</html>";
    }

    private static string GetMedicalOrganizationContractTemplate()
    {
        return @"<!doctype html>
<html lang=""vi"">
  <head>
    <meta charset=""UTF-8"" />
    <title>Hợp đồng Organization - Final Legal</title>
    <style>
      body {
        font-family: ""Times New Roman"", serif;
        background: #f0f0f0;
        padding: 40px;
      }
      .contract-paper {
        background: white;
        width: 210mm;
        min-height: 297mm;
        margin: 0 auto;
        padding: 20mm 25mm;
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        font-size: 13pt;
        line-height: 1.5;
        text-align: justify;
      }
      .center-bold {
        text-align: center;
        font-weight: bold;
        text-transform: uppercase;
        margin: 0;
      }
      .tieu-ngu {
        text-align: center;
        font-weight: bold;
        margin-top: 5px;
        margin-bottom: 20px;
      }
      .tieu-ngu span {
        border-bottom: 1px solid black;
        padding-bottom: 3px;
      }
      .contract-name {
        text-align: center;
        font-size: 16pt;
        font-weight: bold;
        margin-top: 30px;
        margin-bottom: 5px;
        text-transform: uppercase;
      }
      .italic {
        font-style: italic;
      }
      .bold {
        font-weight: bold;
      }
      .section-title {
        font-weight: bold;
        margin-top: 20px;
        margin-bottom: 10px;
        text-transform: uppercase;
      }
      .info-row {
        margin-bottom: 5px;
        padding-left: 20px;
      }
      .checkbox-container {
        display: flex;
        gap: 30px;
        margin: 15px 0;
        padding: 15px;
        border: 1px solid #333;
        background-color: #fafafa;
      }
      ul,
      ol {
        margin-top: 5px;
        margin-bottom: 5px;
        padding-left: 30px;
      }
    </style>
  </head>
  <body>
    <div class=""contract-paper"">
      <div class=""center-bold"">CỘNG HOÀ XÃ HỘI CHỦ NGHĨA VIỆT NAM</div>
      <div class=""tieu-ngu""><span>Độc lập - Tự do - Hạnh phúc</span></div>

      <div class=""contract-name"">HỢP ĐỒNG LIÊN KẾT CUNG CẤP DỊCH VỤ Y TẾ</div>
      <div style=""text-align: center; font-style: italic; margin-bottom: 20px"">
        Số: {{contract_code}}/HĐLK-AURA
      </div>

      <div class=""italic"">
        - Căn cứ Bộ luật Dân sự số 91/2015/QH13 ngày 24/11/2015;<br />
        - Căn cứ Luật Thương mại số 36/2005/QH11 ngày 14/06/2005;<br />
        - Căn cứ Luật Khám bệnh, chữa bệnh số 15/2023/QH15;<br />
        - Căn cứ Nghị định 98/2021/NĐ-CP về quản lý trang thiết bị y tế;<br />
        - Căn cứ Nghị định 13/2023/NĐ-CP về bảo vệ dữ liệu cá nhân;<br />
        - Căn cứ vào năng lực cơ sở vật chất của Bên B và nền tảng công nghệ của
        Bên A.
      </div>

      <p>Hôm nay, ngày {{day}} tháng {{month}} năm {{year}}, chúng tôi gồm:</p>

      <div style=""margin-bottom: 20px"">
        <div class=""bold"">
          BÊN A: CÔNG TY CỔ PHẦN CÔNG NGHỆ AURA (Đơn vị cung cấp Nền tảng)
        </div>
        <div class=""info-row"">
          Địa chỉ trụ sở: Khu Công nghệ cao, Thành phố Thủ Đức, TP.HCM.
        </div>
        <div class=""info-row"">Mã số doanh nghiệp: 031XXXXXXX</div>
        <div class=""info-row"">
          Đại diện theo pháp luật: Ông
          <span class=""bold"">Nguyễn Văn Admin</span>
        </div>
        <div class=""info-row"">Chức vụ: Giám đốc Điều hành</div>
        <div class=""info-row"">
          Điện thoại: 0909XXXXXX &nbsp;&nbsp;|&nbsp;&nbsp; Email:
          partnership@aura.health
        </div>
      </div>

      <div style=""margin-bottom: 20px"">
        <div class=""bold"">BÊN B: {{org_name}} (Cơ sở Y tế)</div>
        <div class=""info-row"">Địa chỉ hoạt động: {{address}}</div>
        <div class=""info-row"">
          Giấy phép hoạt động KCB/Mã số thuế:
          <span class=""bold"">{{tax_id}}</span>
        </div>
        <div class=""info-row"">
          Đại diện theo pháp luật: Ông/Bà
          <span class=""bold"">{{representative_name}}</span>
        </div>
        <div class=""info-row"">Chức vụ: {{representative_role}}</div>
        <div class=""info-row"">
          Điện thoại: {{phone}} &nbsp;&nbsp;|&nbsp;&nbsp; Email: {{email}}
        </div>
        <div class=""info-row"">
          Số tài khoản ngân hàng: {{bank_account}} tại {{bank_name}}
        </div>
      </div>

      <p class=""bold"">
        Bên B xác nhận quy mô và tư cách pháp nhân tham gia liên kết (đánh dấu
        X):
      </p>
      <div class=""checkbox-container"">
        <div style=""flex: 1"">
          <span style=""font-size: 22px"">{{check_clinic}}</span>
          <strong>PHÒNG KHÁM (CLINIC)</strong><br />
          <span class=""italic"" style=""font-size: 11pt""
            >(Quy mô phòng khám đa khoa/chuyên khoa mắt)</span
          >
        </div>
        <div style=""flex: 1"">
          <span style=""font-size: 22px"">{{check_hospital}}</span>
          <strong>BỆNH VIỆN (HOSPITAL)</strong><br />
          <span class=""italic"" style=""font-size: 11pt""
            >(Cơ sở y tế có chức năng điều trị nội trú)</span
          >
        </div>
      </div>

      <p>
        Hai bên cùng thỏa thuận ký kết Hợp đồng liên kết với các điều khoản chi
        tiết sau đây:
      </p>

      <div class=""section-title"">ĐIỀU 1. MỤC TIÊU VÀ NỘI DUNG LIÊN KẾT</div>
      <p>
        1.1. Hai bên hợp tác triển khai dịch vụ
        <strong
          >""Quản lý hồ sơ và Sàng lọc Bệnh lý Võng mạc trên nền tảng số""</strong
        >.
      </p>
      <p>
        1.2. Bên A cung cấp giải pháp chuyển đổi số toàn diện (SaaS). Bên B đóng
        vai trò là đơn vị cung cấp dịch vụ y tế trực tiếp, quản lý hồ sơ bệnh án
        và vận hành thiết bị cận lâm sàng.
      </p>

      <div class=""section-title"">ĐIỀU 2. TRÁCH NHIỆM VÀ NGHĨA VỤ CỦA BÊN B</div>

      <p><strong>2.1. Về Trang thiết bị Y tế (Máy chụp đáy mắt):</strong></p>
      <ul>
        <li>
          Cam kết thiết bị sử dụng (Fundus Camera) phải có
          <strong>Số lưu hành (Giấy phép nhập khẩu)</strong> do Bộ Y tế cấp theo
          quy định tại Nghị định 98/2021/NĐ-CP.
        </li>
        <li>
          Thực hiện bảo trì, bảo dưỡng và hiệu chuẩn thiết bị định kỳ theo
          khuyến cáo của nhà sản xuất để đảm bảo chất lượng hình ảnh đầu ra
          (DICOM/JPG) chính xác nhất cho việc chẩn đoán.
        </li>
      </ul>

      <p><strong>2.2. Về Quản lý Hồ sơ và Vận hành:</strong></p>
      <ul>
        <li>
          Chịu trách nhiệm khởi tạo, lưu trữ và quản lý hồ sơ sức khỏe điện tử
          (EHR) của bệnh nhân trên hệ thống AURA một cách chính xác và bảo mật.
        </li>
        <li>
          Phối hợp với hệ thống để điều phối lịch hẹn (Booking), đảm bảo tiếp
          nhận bệnh nhân đúng khung giờ đã đặt trên ứng dụng, giảm thiểu thời
          gian chờ đợi.
        </li>
        <li>
          Bố trí khu vực và thiết bị (máy tính/máy tính bảng) để hỗ trợ bệnh
          nhân thực hiện các phiên
          <strong>Tư vấn trực tuyến (Online Consultation)</strong> với Bác sĩ
          chuyên gia của AURA khi có yêu cầu.
        </li>
      </ul>

      <div class=""section-title"">ĐIỀU 3. CƠ CHẾ TÀI CHÍNH VÀ THANH TOÁN</div>
      <p>
        3.1. Doanh thu từ phí dịch vụ (sau khi trừ thuế GTGT) được phân chia dựa
        trên đóng góp tài nguyên:
      </p>
      <ul>
        <li>
          <strong>Bên B hưởng: {{clinic_share_rate}}%</strong> (Chi phí khấu hao
          thiết bị, nhân sự, quản lý hồ sơ).
        </li>
        <li>
          <strong>Bên A hưởng:</strong> Phần còn lại (Chi phí bản quyền phần
          mềm, phí kết nối chuyên gia, marketing).
        </li>
      </ul>
      <p>
        3.2. Đối soát tự động hàng tháng. Thanh toán chuyển khoản trong vòng 05
        ngày làm việc sau khi hai bên xác nhận đối soát.
      </p>

      <div class=""section-title"">ĐIỀU 4. CAM KẾT BẢO MẬT THÔNG TIN (NDA)</div>
      <p>
        Hai bên cam kết tuân thủ nghiêm ngặt
        <strong>Nghị định 13/2023/NĐ-CP</strong>. Dữ liệu y tế chỉ được sử dụng
        cho mục đích khám chữa bệnh. Bên B chịu trách nhiệm bảo đảm an toàn
        thông tin tại điểm tiếp nhận, không để lộ lọt dữ liệu bệnh nhân cho bên
        thứ ba.
      </p>

      <div class=""section-title"">ĐIỀU 5. PHẠT VI PHẠM</div>
      <p>
        Bên nào vi phạm nghĩa vụ gây thiệt hại cho bên kia phải bồi thường toàn
        bộ thiệt hại thực tế và chịu phạt vi phạm bằng
        <strong>8% giá trị phần nghĩa vụ bị vi phạm</strong>.
      </p>

      <div class=""section-title"">ĐIỀU 6. ĐIỀU KHOẢN THI HÀNH</div>
      <p>
        6.1. Hợp đồng này có hiệu lực <strong>01 (một) năm</strong> kể từ ngày
        ký. Tự động gia hạn nếu không có văn bản chấm dứt.
      </p>
      <p>
        6.2. Hợp đồng được lập dưới dạng Dữ liệu điện tử, được ký số bởi người
        đại diện theo pháp luật của hai bên và có giá trị pháp lý như bản gốc.
      </p>

      <table style=""width: 100%; margin-top: 50px"">
        <tr>
          <td style=""text-align: center; width: 50%; vertical-align: top"">
            <div class=""bold"">ĐẠI DIỆN BÊN A</div>
            <div class=""italic"">(Ký số hệ thống)</div>
            <br /><br /><br />
            <div
              style=""
                border: 2px solid #d32f2f;
                color: #d32f2f;
                display: inline-block;
                padding: 10px;
                font-weight: bold;
                transform: rotate(-5deg);
              ""
            >
              AURA CORP<br />APPROVED
            </div>
          </td>
          <td style=""text-align: center; width: 50%; vertical-align: top"">
            <div class=""bold"">ĐẠI DIỆN BÊN B</div>
            <div class=""italic"">(Ký tên và Đóng dấu)</div>
            <br /><br />
            <img src=""{{signature_url}}"" alt=""Dấu mộc"" style=""height: 100px"" />
            <div class=""bold"" style=""margin-top: 10px"">
              {{representative_name}}
            </div>
          </td>
        </tr>
      </table>
    </div>
  </body>
</html>";
    }
}
