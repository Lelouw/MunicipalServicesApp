# City of Jozi — Municipal Services (AAPD7112W Part 1)

**Slogan:** *Your city. Your voice. Better service for every Joburger.*

C# .NET Framework **Windows Forms** application for **City of Jozi**. Part 1 implements **Report Issues**, a community reports list, and issue details. Local Events and Service Status appear on the main menu but stay disabled until Part 2.

If this repository is the app folder only, the Task 1 research document may live in the parent course folder: `../Task1_Research/AAPD7112W_Part1_Task1_Research.docx`.

## Features

- **City of Jozi branding** across the main menu, report form, and About dialog
- **Report an issue** with location, category, description, and optional file attachment
- **Live South African place suggestions** while typing a location (OpenStreetMap Nominatim; needs internet)
- **Engagement progress bar** with encouraging messages as the form is completed
- **Inline field validation** (no popup for missing location, category, or short description)
- **Duplicate-report warning** when a similar open issue already exists
- **Community reports** table with search, status filter, attachment column, and status badges
- **Eye icon / double-click** to open full issue details (including attachment preview or open file)
- **In-memory `List<IssueReport>`** storage via `IssueRepository` (Part 1 data-structure requirement)

## Requirements

- Windows 10 or later
- [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48) (included with recent Windows)
- Visual Studio 2019/2022 with the **.NET desktop development** workload, or the .NET SDK with targeting packs for `net48`

## How to compile and run

### Visual Studio

1. Open `MunicipalServicesApp.sln`.
2. Confirm the startup project is **MunicipalServicesApp**.
3. Press **F5** (Debug) or **Ctrl+F5** (Start without debugging).

### Command line

From this folder:

```bat
dotnet build MunicipalServicesApp.sln -c Debug
dotnet run --project MunicipalServicesApp\MunicipalServicesApp.csproj
```

Compiled executable:

`MunicipalServicesApp\bin\Debug\net48\MunicipalServicesApp.exe`

> **Note:** Some work PCs block newly built `.exe` files with Microsoft Defender ASR. If you get **Access is denied** on F5, ask IT to exclude your project folder, or run on a machine without that policy.

## How to use the application

1. The **main menu** opens on startup (City of Jozi header and community reports).
2. Click **Report Issues**.
3. Enter a **location** (type 3+ letters for place suggestions), choose a **category**, and write a **description** (at least 10 characters).
4. Optionally click **Attach file** and select an image or document.
5. Watch the **progress bar** update as you complete each step.
6. Click **Submit**. After confirmation you return to the main menu; the new report appears in **Community reports**.
7. Use search / status filters, or click the **eye** icon to view full details and the attachment.
8. Use **Back to main menu** from the report form if you leave without submitting.

**Local Events** and **Service Status** are disabled on purpose for Part 1.

## Data handling

Reports are stored in a **`List<IssueReport>`** inside `IssueRepository` (singleton). This matches the Part 1 data-structures requirement. Storage is **in memory for the current run** — closing the app clears newly submitted reports; sample seed data loads again on the next start. Attachment files are not copied into the app; only the file path is stored.

## Project structure

```
MunicipalServicesApp/
├── MunicipalServicesApp.sln
└── MunicipalServicesApp/
    ├── Forms/                 # MainMenu, ReportIssues, IssueDetails
    ├── Models/                # IssueReport, LocationSuggestion
    ├── Data/                  # IssueRepository (List storage)
    ├── Services/              # Location search (Nominatim)
    ├── MunicipalityBrand.cs   # City of Jozi name and slogan
    ├── UITheme.cs
    └── Program.cs
```

## Part 2

Keep a copy of this project. Part 2 and the final PoE build on the same application (local events / announcements and service-request status).
