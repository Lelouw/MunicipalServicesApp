# City of Jozi — Municipal Services (AAPD7112W Part 2)

**Slogan:** *Your city. Your voice. Better service for every Joburger.*

C# .NET Framework **Windows Forms** application for **City of Jozi**. Part 1 **Report Issues** stays in place. Part 2 adds **Local Events and Announcements**, with search, advanced data structures, and recommendations based on what you search for.

**Service Request Status** stays on the menu and is disabled until Task 3.

If this repository is the app folder only, open `MunicipalServicesApp.sln` in this folder.

## Features

- **Report Issues** (Part 1): location suggestions, categories, description, optional attachment, progress bar, community reports, and issue details
- **Local Events and Announcements** (Part 2):
  - Upcoming events and announcements as cards, in date order
  - **Priority notices** — urgent announcements first (priority queue)
  - **Issued order** — the same announcements in the order they were published (queue)
  - Search by keyword, category, and date
  - **Recommended for you**, based on the categories, keywords, and dates you search
  - **Recently viewed**, most recent first (stack)
- In-memory storage for the current run. Closing the app clears new issue reports and search history. Sample events load again on the next start.

## Data structures

| Structure | Where it is used |
| --- | --- |
| `List<IssueReport>` | Part 1 community reports |
| `SortedDictionary<DateTime, List<LocalEvent>>` | Events grouped by date. A date search stops once it passes the end date. |
| `Dictionary<string, LocalEvent>` | Hash-table lookup of an event by its id |
| `Dictionary<string, int>` | How often each category and keyword is searched |
| `HashSet<string>` | Unique categories for the filter |
| `HashSet<DateTime>` | Unique event dates, including the “no events on this day” message |
| `Queue<LocalEvent>` | Announcements in the order they were issued |
| `Queue<EventSearch>` | Recent searches, oldest first |
| `Stack<string>` | Recently opened events, most recent on top |
| `EventPriorityQueue` | Binary max-heap. .NET Framework 4.8 has no built-in priority queue, so urgent notices are ordered here. |

Recommendations score each event from those search counts: same category, a related category, a keyword in the title or details, and dates just outside the range you searched. The reason is shown on each recommendation card.

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

1. The **main menu** opens on startup.
2. **Report Issues** works as in Part 1. Submit a report and it appears under **Community reports**. Use the eye icon for full details.
3. Click **Local Events** (the card or **Services → Local Events and Announcements**).
4. Read **Priority notices** across the top. The most urgent notice is first. The side panel **Issued order** lists those same notices in the order they were published, which is not always the urgency order.
5. Browse **Upcoming events**. Click a card for the full description.
6. Search:
   - Type a place, title, or word and click **Search** (or press Enter).
   - Choose a **category**. The list is the unique set of categories on the calendar.
   - Set **From** and **To**. Uncheck a date to ignore that end of the range. The default **From** is today, so past events stay hidden until you move that date back.
7. After you search, **Recommended for you** updates. Each card says why it was suggested, for example “You searched Water” or “Related to Parks and Recreation”.
8. Open a few events. They appear under **Recently viewed**, most recent first.
9. **Clear** resets the filters only. It does not wipe the search history behind the recommendations.
10. **Service Status** stays disabled until Task 3.

## Project structure

```
MunicipalServicesApp/
├── MunicipalServicesApp.sln
└── MunicipalServicesApp/
    ├── Forms/                 # Main menu, Report Issues, Local Events, details
    ├── Models/                # IssueReport, LocalEvent, search and recommendation models
    ├── Data/                  # IssueRepository, EventRepository
    ├── DataStructures/        # EventPriorityQueue (binary heap)
    ├── Services/              # Location search, event recommendations
    ├── MunicipalityBrand.cs
    ├── UITheme.cs
    └── Program.cs
```

## Task 3

Keep a copy of this project. Service Request Status is the remaining main-menu item and is built in Task 3 on this same application.
