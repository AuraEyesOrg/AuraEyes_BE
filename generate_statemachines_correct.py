#!/usr/bin/env python3
"""
Generate state machines using EXACT format from user's XML example
"""

import os


def create_state_machine_xml(flow_id, title, states, transitions):
    """
    Generate XML matching user's exact format
    """
    
    clean_id = flow_id.replace('.', '_')
    
    # Layout config matching user's example
    state_width = 120
    state_height = 60
    vertical_spacing = 110
    x_center = 390
    start_y = 180
    
    # Calculate state positions
    state_positions = {}
    for i, state in enumerate(states):
        state_positions[state] = {
            'x': x_center - state_width // 2,
            'y': start_y + i * vertical_spacing,
        }
    
    start_x = x_center - 15
    start_y_pos = 90
    
    last_y = max(pos['y'] for pos in state_positions.values())
    end_y = last_y + vertical_spacing + 15
    
    # Start XML - EXACT format from user's example
    xml = f'''<mxGraphModel dx="3427" dy="2004" grid="1" gridSize="10" guides="1" tooltips="1" connect="1" arrows="1" fold="1" page="1" pageScale="1" pageWidth="850" pageHeight="1100" math="0" shadow="0">
  <root>
    <mxCell id="0" />
    <mxCell id="1" parent="0" />
'''
    
    # Generate unique IDs for cells
    cell_counter = 1
    state_ids = {}
    edge_ids = {}
    
    # Add start state
    start_id = f"cell_{cell_counter}"
    cell_counter += 1
    
    xml += f'''    <mxCell id="{start_id}" parent="1" style="ellipse;html=1;shape=startState;fillColor=#000000;strokeColor=#ff0000;" value="" vertex="1">
      <mxGeometry height="30" width="30" x="{start_x}" y="{start_y_pos}" as="geometry" />
    </mxCell>
'''
    
    # Add edge from start to first state (will add later after we have first state ID)
    start_edge_id = f"cell_{cell_counter}"
    cell_counter += 1
    
    # Add all states
    for i, state in enumerate(states):
        state_id = f"cell_{cell_counter}"
        state_ids[state] = state_id
        cell_counter += 1
        
        pos = state_positions[state]
        xml += f'''    <mxCell id="{state_id}" parent="1" style="rounded=1;whiteSpace=wrap;html=1;" value="{state}" vertex="1">
      <mxGeometry height="{state_height}" width="{state_width}" x="{pos['x']}" y="{pos['y']}" as="geometry" />
    </mxCell>
'''
    
    # Add end state
    end_id = f"cell_{cell_counter}"
    cell_counter += 1
    
    xml += f'''    <mxCell id="{end_id}" parent="1" style="ellipse;html=1;shape=endState;fillColor=#000000;strokeColor=#ff0000;" value="" vertex="1">
      <mxGeometry height="30" width="30" x="{start_x}" y="{end_y}" as="geometry" />
    </mxCell>
'''
    
    # Add edge from start to first state
    if states:
        first_state_id = state_ids[states[0]]
        xml += f'''    <mxCell id="{start_edge_id}" edge="1" parent="1" source="{start_id}" style="edgeStyle=orthogonalEdgeStyle;html=1;verticalAlign=bottom;endArrow=open;endSize=8;strokeColor=#000000;rounded=0;" value="">
      <mxGeometry relative="1" as="geometry">
        <mxPoint x="{x_center}" y="{start_y}" as="targetPoint" />
      </mxGeometry>
    </mxCell>
'''
    
    # Add transitions between states
    for from_state, to_state, label in transitions:
        if from_state in state_ids and to_state in state_ids:
            edge_id = f"cell_{cell_counter}"
            cell_counter += 1
            
            from_id = state_ids[from_state]
            to_id = state_ids[to_state]
            
            xml += f'''    <mxCell id="{edge_id}" edge="1" parent="1" source="{from_id}" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;exitX=0.5;exitY=1;exitDx=0;exitDy=0;endArrow=open;endFill=0;" target="{to_id}">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
'''
            
            # Add label
            label_id = f"cell_{cell_counter}"
            cell_counter += 1
            
            from_y = state_positions[from_state]['y']
            label_y = from_y + state_height + 10
            label_x = x_center + state_width // 2 + 10
            
            xml += f'''    <mxCell id="{label_id}" parent="1" style="text;whiteSpace=wrap;html=1;" value="{label}" vertex="1">
      <mxGeometry height="30" width="150" x="{label_x}" y="{label_y}" as="geometry" />
    </mxCell>
'''
    
    # Add edge from last state to end
    if states:
        last_state_id = state_ids[states[-1]]
        final_edge_id = f"cell_{cell_counter}"
        cell_counter += 1
        
        xml += f'''    <mxCell id="{final_edge_id}" edge="1" parent="1" source="{last_state_id}" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;exitX=0.5;exitY=1;exitDx=0;exitDy=0;endArrow=open;endFill=0;" target="{end_id}">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
'''
    
    # Add title at bottom
    title_id = f"cell_{cell_counter}"
    xml += f'''    <mxCell id="{title_id}" parent="1" style="text;html=1;whiteSpace=wrap;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;rounded=0;fontFamily=Helvetica;fontSize=11;fontColor=default;labelBackgroundColor=default;" value="state machine for {title.lower()}" vertex="1">
      <mxGeometry height="30" width="60" x="{x_center - 30}" y="{end_y + 70}" as="geometry" />
    </mxCell>
'''
    
    xml += '''  </root>
</mxGraphModel>
'''
    
    return xml


# State machines data
state_machines = [
    {"id": "3.1.1", "title": "Register Patient Account", "states": ["Unverified", "EmailSent", "Verified", "Active"], "transitions": [("Unverified", "EmailSent", "send verification email"), ("EmailSent", "Verified", "user clicks link"), ("Verified", "Active", "complete profile"), ("EmailSent", "Unverified", "email expired / resend")]},
    {"id": "3.1.2", "title": "Patient Login", "states": ["Unauthenticated", "Authenticating", "Authenticated", "SessionActive"], "transitions": [("Unauthenticated", "Authenticating", "submit credentials"), ("Authenticating", "Authenticated", "credentials valid"), ("Authenticating", "Unauthenticated", "credentials invalid"), ("Authenticated", "SessionActive", "create session")]},
    {"id": "3.1.3", "title": "Internal Staff Secure Login", "states": ["Unauthenticated", "Authenticating", "Authenticated", "SessionActive"], "transitions": [("Unauthenticated", "Authenticating", "submit credentials"), ("Authenticating", "Authenticated", "credentials valid"), ("Authenticating", "Unauthenticated", "credentials invalid"), ("Authenticated", "SessionActive", "create session")]},
    {"id": "3.1.4", "title": "Password Reset Flow", "states": ["RequestReset", "TokenSent", "TokenValidated", "PasswordReset"], "transitions": [("RequestReset", "TokenSent", "send reset email"), ("TokenSent", "TokenValidated", "user clicks link & token valid"), ("TokenSent", "RequestReset", "token expired"), ("TokenValidated", "PasswordReset", "submit new password")]},
    {"id": "3.1.5", "title": "Two-Factor Authentication", "states": ["Disabled", "SetupInitiated", "QRGenerated", "CodeVerified", "Enabled"], "transitions": [("Disabled", "SetupInitiated", "user enables 2FA"), ("SetupInitiated", "QRGenerated", "generate secret & QR"), ("QRGenerated", "CodeVerified", "user enters valid code"), ("CodeVerified", "Enabled", "activate 2FA")]},
    {"id": "3.1.6", "title": "Internal Account Management", "states": ["Created", "Active", "Suspended", "Deleted"], "transitions": [("Created", "Active", "admin activates"), ("Active", "Suspended", "admin suspends"), ("Suspended", "Active", "admin reactivates"), ("Active", "Deleted", "admin deletes")]},
    {"id": "3.2.4", "title": "Leave Request Submission", "states": ["Pending", "Approved", "Rejected", "Cancelled"], "transitions": [("Pending", "Approved", "admin approves"), ("Pending", "Rejected", "admin rejects"), ("Pending", "Cancelled", "ophthalmologist cancels")]},
    {"id": "3.2.5", "title": "Leave Request Processing", "states": ["Pending", "UnderReview", "Approved", "Rejected"], "transitions": [("Pending", "UnderReview", "admin reviews"), ("UnderReview", "Approved", "admin approves"), ("UnderReview", "Rejected", "admin rejects")]},
    {"id": "3.3.2", "title": "Appointment Scheduling", "states": ["Pending", "Confirmed", "CheckedIn", "InProgress", "Completed"], "transitions": [("Pending", "Confirmed", "clinic confirms"), ("Confirmed", "CheckedIn", "patient checks in"), ("CheckedIn", "InProgress", "consultation starts"), ("InProgress", "Completed", "consultation ends")]},
    {"id": "3.3.3", "title": "Visit Lifecycle", "states": ["CheckedIn", "InProgress", "WaitingForPayment", "Completed"], "transitions": [("CheckedIn", "InProgress", "doctor starts consultation"), ("InProgress", "WaitingForPayment", "doctor completes consultation"), ("WaitingForPayment", "Completed", "payment completed")]},
    {"id": "3.3.4", "title": "Clinic Queue Management", "states": ["InQueue", "Assigned", "InProgress", "Completed"], "transitions": [("InQueue", "Assigned", "staff assigns to doctor"), ("Assigned", "InProgress", "doctor starts consultation"), ("InProgress", "Completed", "consultation completed")]},
    {"id": "3.3.6", "title": "Appointment Slot Management", "states": ["Available", "Blocked", "Deleted"], "transitions": [("Available", "Blocked", "staff blocks slot"), ("Blocked", "Available", "staff unblocks slot")]},
    {"id": "3.4.1", "title": "Retinal Photo Upload", "states": ["Initiated", "Uploaded", "Validated", "Ready"], "transitions": [("Initiated", "Uploaded", "upload image(s)"), ("Uploaded", "Validated", "validate format & size"), ("Validated", "Ready", "create screening session")]},
    {"id": "3.4.2", "title": "Patient Consent Capture", "states": ["NotProvided", "Pending", "Provided", "Declined"], "transitions": [("NotProvided", "Pending", "request consent"), ("Pending", "Provided", "patient accepts"), ("Pending", "Declined", "patient declines")]},
    {"id": "3.4.3", "title": "AI Analysis Workflow", "states": ["Initiated", "Analyzing", "Completed", "Failed"], "transitions": [("Initiated", "Analyzing", "send to AI service"), ("Analyzing", "Completed", "AI returns result"), ("Analyzing", "Failed", "timeout or error")]},
    {"id": "3.4.5", "title": "Clinic Screening Operations", "states": ["Created", "ImageUploaded", "AIAnalyzing", "ResultReady", "DoctorReviewed"], "transitions": [("Created", "ImageUploaded", "staff uploads images"), ("ImageUploaded", "AIAnalyzing", "trigger AI"), ("AIAnalyzing", "ResultReady", "AI completes"), ("ResultReady", "DoctorReviewed", "doctor reviews")]},
    {"id": "3.4.6", "title": "Post-Visit Chat Session", "states": ["Pending", "Confirmed", "Active", "Completed"], "transitions": [("Pending", "Confirmed", "doctor confirms"), ("Confirmed", "Active", "session starts"), ("Active", "Completed", "session ends")]},
    {"id": "3.4.7", "title": "Consultation Chat Status", "states": ["Locked", "MemoOnly", "Open", "Archived"], "transitions": [("Locked", "Open", "doctor unlocks"), ("Open", "MemoOnly", "restrict to memo"), ("MemoOnly", "Open", "allow full chat")]},
    {"id": "3.4.8", "title": "Medical Diagnosis Creation", "states": ["Draft", "UnderReview", "Finalized", "Amended"], "transitions": [("Draft", "UnderReview", "doctor submits"), ("UnderReview", "Finalized", "doctor finalizes"), ("UnderReview", "Draft", "doctor revises")]},
    {"id": "3.4.9", "title": "Patient Health Roadmap", "states": ["Upcoming", "InProgress", "Completed", "Cancelled"], "transitions": [("Upcoming", "InProgress", "step date arrives"), ("InProgress", "Completed", "patient completes step")]},
    {"id": "3.5.2", "title": "Payment Order Creation", "states": ["Pending", "Confirmed", "Processing", "Completed"], "transitions": [("Pending", "Confirmed", "staff confirms order"), ("Confirmed", "Processing", "payment initiated"), ("Processing", "Completed", "payment successful")]},
    {"id": "3.5.3", "title": "Transaction Processing", "states": ["Pending", "Processing", "Completed", "Failed"], "transitions": [("Pending", "Processing", "initiate payment"), ("Processing", "Completed", "payment successful"), ("Processing", "Failed", "payment failed")]},
    {"id": "3.5.4", "title": "Payment Webhook Processing", "states": ["Received", "Validating", "Processed", "Failed"], "transitions": [("Received", "Validating", "validate webhook"), ("Validating", "Processed", "webhook valid"), ("Validating", "Failed", "webhook invalid")]},
    {"id": "3.6.3", "title": "EMR Clinical Forms", "states": ["Draft", "InProgress", "Completed", "Archived"], "transitions": [("Draft", "InProgress", "doctor starts form"), ("InProgress", "Completed", "doctor completes form"), ("Completed", "Archived", "archive form")]},
    {"id": "3.6.4", "title": "Post-Visit Follow-up Initiation", "states": ["NotInitiated", "Requested", "Scheduled", "Completed"], "transitions": [("NotInitiated", "Requested", "patient requests"), ("Requested", "Scheduled", "doctor schedules"), ("Scheduled", "Completed", "follow-up completed")]},
    {"id": "3.6.5", "title": "Post-Visit Follow-up Response", "states": ["Pending", "InProgress", "Responded", "Closed"], "transitions": [("Pending", "InProgress", "doctor starts response"), ("InProgress", "Responded", "doctor sends response"), ("Responded", "Closed", "patient acknowledges")]},
    {"id": "3.7.1", "title": "Clinical Case Sharing", "states": ["Draft", "Published", "Archived", "Deleted"], "transitions": [("Draft", "Published", "doctor publishes"), ("Published", "Archived", "doctor archives")]},
    {"id": "3.7.3", "title": "Internal Group Chat", "states": ["Created", "Active", "Inactive", "Archived"], "transitions": [("Created", "Active", "members join"), ("Active", "Inactive", "no activity"), ("Inactive", "Active", "new message")]},
    {"id": "3.7.5", "title": "Content Moderation", "states": ["Published", "Flagged", "UnderReview", "Approved", "Removed"], "transitions": [("Published", "Flagged", "user reports"), ("Flagged", "UnderReview", "admin reviews"), ("UnderReview", "Approved", "admin approves")]},
    {"id": "3.8.5", "title": "Notification Inbox", "states": ["Unread", "Read", "Archived", "Deleted"], "transitions": [("Unread", "Read", "user reads"), ("Read", "Archived", "user archives")]},
    {"id": "3.8.6", "title": "Notification Delivery", "states": ["Queued", "Sending", "Delivered", "Failed"], "transitions": [("Queued", "Sending", "process notification"), ("Sending", "Delivered", "delivery successful"), ("Sending", "Failed", "delivery failed")]},
]


def generate_all_files():
    """Generate all state machine XML files"""
    
    print("Generating state machine XML files (EXACT user format)...")
    
    script_dir = os.path.dirname(os.path.abspath(__file__))
    output_dir = os.path.join(script_dir, "statemachines")
    
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)
    
    for sm in state_machines:
        print(f"  - {sm['id']} {sm['title']}")
        
        xml_content = create_state_machine_xml(
            sm['id'],
            sm['title'],
            sm['states'],
            sm['transitions']
        )
        
        filename = f"{sm['id'].replace('.', '_')}_{sm['title'].replace(' ', '_').replace('&', 'and').replace('/', '_')}.xml"
        output_file = os.path.join(output_dir, filename)
        
        with open(output_file, "w", encoding="utf-8") as f:
            f.write(xml_content)
    
    print(f"\n[OK] Generated {len(state_machines)} XML files")
    print(f"  - Format: EXACT match to user's example")
    print(f"  - Output: {output_dir}")


if __name__ == "__main__":
    generate_all_files()
