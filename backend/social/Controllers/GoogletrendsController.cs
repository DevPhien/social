using GoogleApi;
using GoogleApi.Entities.Maps.AerialView.Common;
using System.Collections;

namespace social.Controllers;

[ApiController]
[Route("api/googletrends")]
[ApiExplorerSettings(GroupName = "Googletrends")]
public class GoogletrendsController : Controller
{
    [HttpGet("getTrendTimeSeriesMultiple")]
    public async Task<IActionResult> getTrendTimeSeriesMultiple(string keywords)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "engine", "google_trends" },
            { "q", keywords },
            { "data_type", "TIMESERIES" },
        };

        var _uriBuilder = new UriBuilder($"https://serpapi.com/search.json");
        _uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var _httpClient = new HttpClient();
        var response = await _httpClient.GetAsync(_uriBuilder.Uri);
        if (response.IsSuccessStatusCode)
        {
            return Ok(new { error = false, result = await response.Content.ReadAsStringAsync() });
        }
        else
        {
            return Ok(new { error = true, result = "" });
        }
    }

    [HttpGet("getTrendGeoMap")]
    public async Task<IActionResult> getTrendGeoMap(string keyword)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "engine", "google_trends" },
            { "q", keyword },
            { "data_type", "GEO_MAP_0" },
        };

        var _uriBuilder = new UriBuilder($"https://serpapi.com/search.json");
        _uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var _httpClient = new HttpClient();
        var response = await _httpClient.GetAsync(_uriBuilder.Uri);
        if (response.IsSuccessStatusCode)
        {
            return Ok(new { error = false, result = await response.Content.ReadAsStringAsync() });
        }
        else
        {
            return Ok(new { error = true, result = "" });
        }
    }

    [HttpGet("getTrendGeoMapMultiple")]
    public async Task<IActionResult> getTrendGeoMapMultiple(string keywords)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "engine", "google_trends" },
            { "q", keywords },
            { "data_type", "GEO_MAP" },
        };

        var _uriBuilder = new UriBuilder($"https://serpapi.com/search.json");
        _uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var _httpClient = new HttpClient();
        var response = await _httpClient.GetAsync(_uriBuilder.Uri);
        if (response.IsSuccessStatusCode)
        {
            return Ok(new { error = false, result = await response.Content.ReadAsStringAsync() });
        }
        else
        {
            return Ok(new { error = true, result = "" });
        }
    }

    [HttpGet("getTrendRelateTopics")]
    public async Task<IActionResult> getTrendRelateTopics(string keyword)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "engine", "google_trends" },
            { "q", keyword },
            { "data_type", "RELATED_QUERIES" },
        };

        var _uriBuilder = new UriBuilder($"https://serpapi.com/search.json");
        _uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var _httpClient = new HttpClient();
        var response = await _httpClient.GetAsync(_uriBuilder.Uri);
        if (response.IsSuccessStatusCode)
        {
            return Ok(new { error = false, result = await response.Content.ReadAsStringAsync() }) ;
        }
        else
        {
            return Ok(new { error = true, result = "" });
        }
    }

    [HttpGet("getTrendRelateQueries")]
    public async Task<IActionResult> getTrendRelateQueries(string keyword)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "engine", "google_trends" },
            { "q", keyword },
            { "data_type", "RELATED_QUERIES" },
        };

        var _uriBuilder = new UriBuilder($"https://serpapi.com/search.json");
        _uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var _httpClient = new HttpClient();
        var response = await _httpClient.GetAsync(_uriBuilder.Uri);
        if (response.IsSuccessStatusCode)
        {
            return Ok(new { error = false, result = await response.Content.ReadAsStringAsync() });
        }
        else
        {
            return Ok(new { error = true, result = "" });
        }
    }

    [HttpGet("getTrendTimeSeries")]
    public async Task<IActionResult> getTrendTimeSeries(string keyword)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "engine", "google_trends" },
            { "q", keyword },
            { "date", "now 7-d" },
            { "tz", "-540"},
            { "data_type", "RELATED_QUERIES" },
        };

        var _uriBuilder = new UriBuilder($"https://serpapi.com/search.json");
        _uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var _httpClient = new HttpClient();
        var response = await _httpClient.GetAsync(_uriBuilder.Uri);
        if (response.IsSuccessStatusCode)
        {
            return Ok(new { error = false, result = await response.Content.ReadAsStringAsync() });
        }
        else
        {
            return Ok(new { error = true, result = "" });
        }
    }
}
