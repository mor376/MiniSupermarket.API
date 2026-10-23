---
name: my-agent
description: Describe what this custom agent does and when to use it.
---

# my-agent

Define what this custom agent does, including its behavior, capabilities, and any specipublic FormAnalyticsDashboard()
{
    // ... code cũ ...
    _timeFilter.SelectedIndexChanged += async (_, _) => await LoadReportAsync();
    _storeFilter.SelectedIndexChanged += async (_, _) => await LoadReportAsync(); // Thêm dòng này
    Load += async (_, _) => await LoadReportAsync();
}

private async Task LoadReportAsync()
{
    var range = _timeFilter.SelectedItem?.ToString() switch
    {
        "7 ngày" => "7",
        "30 ngày" => "30",
        "90 ngày" => "90",
        _ => "30"
    };

    // Lấy chi nhánh
    var store = _storeFilter.SelectedItem?.ToString() switch
    {
        "Tất cả chi nhánh" => "",
        var s => s // tên chi nhánh, ví dụ: "Chi nhánh An Phước"
    };

    // Thêm tham số chi nhánh vào API nếu có
    var storeParam = string.IsNullOrEmpty(store) ? "" : $"&store={Uri.EscapeDataString(store)}";

    try
    {
        var summaryTask = ApiClientService.GetAsync<DashboardSummaryResponse>($"reports/dashboard-summary?range={range}{storeParam}");
        var chartTask = ApiClientService.GetAsync<List<SalesByDayDto>>($"reports/sales-by-day?range={range}{storeParam}");
        await Task.WhenAll(summaryTask, chartTask);

        // ... code cũ cập nhật UI ...
    }
    catch (Exception ex)
    {
        // ... code cũ ...
    }
}