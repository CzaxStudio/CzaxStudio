#r "nuget: Octokit, 11.0.1"

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

var github = new GitHubClient(new ProductHeaderValue("CzaxStudio-Readme-Updater"));

// Fetch all public repositories for your profile
var repos = await github.Repository.GetAllForUser("CzaxStudio");

// Group and calculate language frequency manually, or pull primary languages
var langCounts = repos
    .Where(r => r.Language != null)
    .GroupBy(r => r.Language)
    .Select(g => new { Language = g.Key, Count = g.Count() })
    .OrderByDescending(g => g.Count);

// Generate your custom text block
string statsMarkdown = "### Automated Language Stats\n";
foreach (var lang in langCounts)
{
    statsMarkdown += $"- **{lang.Language}** ({lang.Count} projects)\n";
}

// Read current README, locate your placeholder, and inject the new stats
string readmePath = "README.md";
string readmeContent = await File.ReadAllTextAsync(readmePath);

// Simple replacement logic (Ensure you have placeholders in your README)
string startMarker = "";
string endMarker = "";

int startIdx = readmeContent.IndexOf(startMarker) + startMarker.Length;
int endIdx = readmeContent.IndexOf(endMarker);

if (startIdx >= 0 && endIdx > startIdx)
{
    string newContent = readmeContent.Substring(0, startIdx) + "\n" + statsMarkdown + readmeContent.Substring(endIdx);
    await File.WriteAllTextAsync(readmePath, newContent);
    Console.WriteLine("README updated successfully with C# script.");
}
