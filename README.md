# City of Jozi — Municipal Services (AAPD7112W)

**Slogan:** *Your city. Your voice. Better service for every Joburger.*

C# .NET Framework **Windows Forms** application for **City of Jozi**. Part 1 is **Report Issues**. Part 2 is **Local Events and Announcements**. Part 3 is **Service Request Status**. Residents do not register. Staff use a desk code.

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
- **New announcement**: staff publish a notice into the same lists as the sample data
- **Service Request Status** (Part 3):
  - Choose **Resident** or **Service desk** every time the page opens
  - A resident can track a reference and read the journey. Update stays hidden
  - A service-desk agent enters a code from the staff list, then uses **Update** in the Action column
  - Status moves one step at a time: Pending, Acknowledged, Assigned, In Progress, Resolved, Closed
  - The page shows the catalogue tree, the search paths, the urgency heap, related jobs, and the depot route
- **Bell** on the main menu. Submitting a report watches it. A status change for a watched reference appears there. No account is created
- In-memory storage for the current run. Closing the app clears new reports, new notices, and search history. Sample data loads again on the next start.

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
| `GeneralTreeNode` | Service catalogue: city, departments, categories, then each request |
| `BinarySearchTree` | Requests by reference. Track writes the comparison path |
| `AvlTree` | Requests by date. A range query counts reports on or after a date |
| `RedBlackTree` | Requests by status. A status change removes the old key and inserts the new one |
| `BinaryHeap` | Open requests scored for the urgency column and Attend to these first |
| `WeightedGraph` | Related jobs, and the depot network. Breadth-first, depth-first, and Dijkstra |
| `UnionFind` | Kruskal minimum spanning tree over the nine depots |
| `HashSet` of watched references | The bell. A status change is sent only for a reference on this set |

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
10. **Local Events → New announcement** asks for the municipal desk, then **Publish** adds the notice to the same lists as the sample data.
11. The sample desk codes are `JOZI-DESK`, `SANDTON-01`, `SOWETO-02`, and `CBD-03` (`MunicipalDeskCodes` in `App.config`).
12. **Service Status** asks whether you are a resident or at the service desk.
    - **Resident**: track a reference such as `5` or `MSR-2026-0005`. You can read the history. You cannot change a status.
    - **Service desk**: enter a code from the list. **Update**, next to Urgency, opens the confirmation and moves that request one step.
13. Open a community report from the eye icon and set **Pending**, **In Progress**, or **Resolved**. That path also asks for the desk.
14. The **bell** shows notices for reports submitted on this computer. You can also type a reference and watch it. No account is created.

## Project structure

```
MunicipalServicesApp/
├── MunicipalServicesApp.sln
└── MunicipalServicesApp/
    ├── Forms/                 # Main menu, reports, events, status, desk, and the bell
    ├── Models/                # IssueReport, LocalEvent, request status, and notifications
    ├── Data/                  # IssueRepository, EventRepository
    ├── DataStructures/        # Trees, heaps, the graph, and the minimum spanning tree
    ├── Services/              # Desk check, catalogue, priorities, notifications, depot network
    ├── MunicipalityBrand.cs
    ├── UITheme.cs
    └── Program.cs
```

Part 3 is in this same application. Service Request Status is open from the main menu.
