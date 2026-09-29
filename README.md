# Blazor DataGrid SQL Server Databinding using SqlClient data provider

This sample demonstrates how to bind the Syncfusion [Blazor DataGrid](https://www.syncfusion.com/blazor-components/blazor-datagrid) to Microsoft SQL Server data using the `Microsoft.Data.SqlClient` provider and a Custom Adaptor. The implementation retrieves records from the `NORTHWND.MDF` database and performs server-side processing through the Grid's `DataManagerRequest` object. The sample overrides the `Read` method of the custom adaptor to generate and execute SQL queries, retrieve data through `SqlDataAdapter`, convert the results into strongly typed objects, and return a `DataResult` that can be consumed directly by the Syncfusion Blazor DataGrid.

```xml
<SfGrid TValue="Order" AllowPaging="true">
    <SfDataManager Adaptor="Adaptors.CustomAdaptor">
        <CustomAdaptorComponent></CustomAdaptorComponent>
    </SfDataManager>
    <GridColumns>
        <GridColumn Field=@nameof(Order.OrderID) HeaderText="Order ID" IsIdentity="true" IsPrimaryKey="true" TextAlign="TextAlign.Right" Width="120">
        </GridColumn>
        <GridColumn Field=@nameof(Order.CustomerID) HeaderText="Customer Name" Width="150"></GridColumn>
    </GridColumns>
</SfGrid>
@code{
    SfGrid<Order> Grid { get; set; }
    public static List<Order> Orders { get; set; }

    public class Order
    {
        public int? OrderID { get; set; }
        public string CustomerID { get; set; }
    }
}
```

```xml
@using Syncfusion.Blazor;
@using Syncfusion.Blazor.Data;
@using Newtonsoft.Json
@using static EFGrid.Pages.Index;
@using Microsoft.Data.SqlClient;
@using System.Data;
@using Microsoft.AspNetCore.Hosting;
@inject IHostingEnvironment _env

@inherits DataAdaptor<Order>

<CascadingValue Value="@this">
    @ChildContent
</CascadingValue>

@code {
    [Parameter]
    [JsonIgnore]
    public RenderFragment ChildContent { get; set; }
    public static DataSet CreateCommand(string queryString, string connectionString)
    {
        using (SqlConnection connection = new SqlConnection(
                   connectionString))
        {

            SqlDataAdapter adapter = new SqlDataAdapter(queryString, connection);
            DataSet dt = new DataSet();
            try
            {
                connection.Open();
                adapter.Fill(dt);// using sqlDataAdapter we process the query string and fill the data into dataset
            }
            catch (SqlException se)
            {
                Console.WriteLine(se.ToString());
            }
            finally
            {
                connection.Close();
            }
            return dt;
        }
    }
    // Performs data Read operation
    public override object Read(DataManagerRequest dm, string key = null)
    {
        string appdata = _env.ContentRootPath;
        string path = Path.Combine(appdata, "App_Data\\NORTHWND.MDF");
        string str = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename='{path}';Integrated Security=True;Connect Timeout=30";        // based on the skip and take count from DataManagerRequest here we formed SQL query string    
        string qs = "SELECT OrderID, CustomerID FROM dbo.Orders ORDER BY OrderID OFFSET " + dm.Skip + " 
ROWS FETCH NEXT " + dm.Take + " ROWS ONLY;";
        DataSet data = CreateCommand(qs, str);
        Orders = data.Tables[0].AsEnumerable().Select(r => new Order
        {
            OrderID = r.Field<int>("OrderID"),
            CustomerID = r.Field<string>("CustomerID")
        }).ToList();  // here we convert dataset into list
        IEnumerable<Order> DataSource = Orders;
        SqlConnection conn = new SqlConnection(str);
        conn.Open();
        SqlCommand comm = new SqlCommand("SELECT COUNT(*) FROM dbo.Orders", conn);
        Int32 count = (Int32)comm.ExecuteScalar();
        return dm.RequiresCounts ? new DataResult() { Result = DataSource, Count = count } : (object)DataSource;
    }
}
```

* In this sample, we handled Paging action for Blazor grid based on your need you can extend the given logic for other operations.
* For performing data manipulation, you can override other methods such as `Insert`, `Update` and `Remove` of Custom Adaptor.

## Key Features

- Uses the Syncfusion Blazor `DataAdaptor` class to implement custom data binding.
- Overrides the `Read(DataManagerRequest dm, string key = null)` method to process Grid requests.
- Uses the `DataManagerRequest` object to obtain paging information such as `Skip` and `Take`.
- Generates SQL Server paging queries using `OFFSET` and `FETCH NEXT`.
- Uses `Microsoft.Data.SqlClient` APIs including:
  - `SqlConnection`
  - `SqlDataAdapter`
  - `SqlCommand`
- Retrieves data into a `DataSet` through `SqlDataAdapter.Fill`.
- Converts database records into a strongly typed `Order` model.
- Returns a `DataResult` object containing both `Result` and `Count` values required by the Grid.
- Uses the `NORTHWND.MDF` database attached through LocalDB.
- Demonstrates server-side paging logic that can be extended for filtering, sorting, CRUD operations, and other DataGrid actions.

## Prerequisites

- Visual Studio 2022 or Visual Studio Code
- .NET SDK compatible with the project's target framework
- Microsoft SQL Server LocalDB
- Microsoft.Data.SqlClient package
- Syncfusion.Blazor package

## How to Run the Project

**Visual Studio 2022**

1. Clone or download this repository.
2. Open the solution file from either:
   - `NET5/`
   - `NET6/EFGrid/`
3. Restore all NuGet packages.
4. Ensure the `NORTHWND.MDF` database is available in the application's `App_Data` folder.
5. Build the solution.
6. Set the corresponding application project as the startup project if required.
7. Run the application using `Ctrl+F5`.

**Visual Studio Code**

1. Open the repository folder in Visual Studio Code.
2. Open the integrated terminal.
3. Navigate to the desired sample project directory.

```bash
cd NET6/EFGrid
dotnet restore
dotnet run
```

Or:

```bash
cd NET5
dotnet restore
dotnet run
```

4. Open the application URL displayed in the console after the application starts.

## Project Structure

- `NET6/EFGrid/Pages/Index.razor` — contains the `SfGrid` implementation, `Order` model, and Grid configuration.
- `NET6/EFGrid/` — contains the .NET 6 sample demonstrating SQL Server binding through a custom adaptor.
- `NET5/` — contains the .NET 5 implementation of the SQL databinding sample.
- `App_Data/NORTHWND.MDF` — SQL Server database used as the sample data source.
- Custom `DataAdaptor` implementation — overrides `Read(DataManagerRequest dm, string key = null)` to execute SQL queries and return Grid data.
- `Microsoft.Data.SqlClient` integration — uses `SqlConnection`, `SqlDataAdapter`, and `SqlCommand` to retrieve records and record counts from SQL Server.

## Support and Feedback

- For general product questions, visit the [Syncfusion Community Forum](https://www.syncfusion.com/forums) or [Syncfusion Support](https://www.syncfusion.com/support).
- To report an issue specific to this sample, open a GitHub issue in this repository.
- For official documentation related to this feature, see https://help.syncfusion.com/grid-sdk/blazor/data-grid/connecting-to-adaptors/custom-adaptor

## License

This is a Syncfusion sample project provided to demonstrate product usage. Review the [Syncfusion license terms](https://www.syncfusion.com/sales/pricing?category=ui-components) before using Syncfusion components in your own applications.
