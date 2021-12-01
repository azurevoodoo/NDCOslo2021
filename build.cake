Task("Restore")
    .Does(() => DotNetBuild("src"));

Task("Build")
    .IsDependentOn("Restore")
    .Does(()=>DotNetBuild("src"));

Task("Test")
    .IsDependentOn("Build")
    .Does(()=>DotNetTest("src"));

Task("Pack")
    .IsDependentOn("Test")
    .Does(()=>DotNetPack("src", new DotNetPackSettings {
        OutputDirectory = "./artifacts"
    }));

Task("Upload-Artifacts")
    .IsDependentOn("Pack")
    .Does(()=>GitHubActions.Commands.UploadArtifact(
        Directory("./artifacts"),
        "NuGet"
    ));

RunTarget(Argument("target", "Build"));