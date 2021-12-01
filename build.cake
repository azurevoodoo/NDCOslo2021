public record BuildData(
    DirectoryPath Project,
    DirectoryPath Artifacts
    );

Setup(
    context=>  new BuildData(
        "src",
        context.MakeAbsolute(Directory("artifacts"))
    )
);

Task("Restore")
    .Does<BuildData>((context, data) => DotNetRestore(data.Project.FullPath));

Task("Build")
    .IsDependentOn("Restore")
    .Does<BuildData>((context, data) => DotNetBuild(data.Project.FullPath));

Task("Test")
    .IsDependentOn("Build")
    .Does<BuildData>((context, data) => DotNetTest(data.Project.FullPath));

Task("Pack")
    .IsDependentOn("Test")
    .Does<BuildData>((context, data) => DotNetPack("src", new DotNetPackSettings {
        OutputDirectory = data.Artifacts
    }));

Task("Upload-Artifacts")
    .IsDependentOn("Pack")
    .Does(()=>GitHubActions.Commands.UploadArtifact(
        MakeAbsolute(Directory("./artifacts")),
        $"NuGet{Context.Environment.Platform.Family}"
    ));

Task("GitHubActions")
    .IsDependentOn("Upload-Artifacts");

Task("Default")
    .IsDependentOn("Test");

RunTarget(Argument("target", "Default"));