function CreateDirectory {
    param (
        $DirectoryPath
    )

    try {
        "The directory '$DirectoryPath' does not exist, this needs to be created."
        New-Item -Path $DirectoryPath -ItemType Directory -ErrorAction Stop | Out-Null #-Force
    }
    catch {
        Write-Error -Message "Unable to create directory '$DirectoryPath'. Error was: $_" -ErrorAction Stop 
    }
    "Successfully created directory '$DirectoryPath'."

}

###################################################### API Plugins ######################################################

# Creating directories needed for extracting Protobuf and gRPC dependencies.
$CurrentPath = Get-Location
$DownloadPath = Join-Path -Path $CurrentPath -ChildPath "\Temp"
if (-not (Test-Path -LiteralPath $DownloadPath)) {
    CreateDirectory($DownloadPath)
}

# Download the zipped gRPC binaries that are needed. 
Invoke-WebRequest -Uri "https://packages.grpc.io/archive/2019/12/a02d6b9be81cbadb60eed88b3b44498ba27bcba9-edd81ac6-e3d1-461a-a263-2b06ae913c3f/protoc/grpc-protoc_windows_x64-1.26.0-dev.zip" -OutFile ".\Temp\grpc.zip"

# The path to the file to be unzipped and extracted
$GrpcZipPath = Join-Path -Path $CurrentPath -ChildPath "\Temp\grpc.zip" 

# The path to where the contents of the zip file should be extracted
$APIExtractionDir = Join-Path -Path $CurrentPath -ChildPath "\API\Protobuf\Plugins"

# Check whether the Plugins direcotry already exists, if not create it before extracting the zip file.
if (-not (Test-Path -LiteralPath $APIExtractionDir)) {
    CreateDirectory($APIExtractionDir)
    Expand-Archive -LiteralPath $GrpcZipPath -DestinationPath $APIExtractionDir 
}
else {
    "The directory '$APIExtractionDir' already exists, stopped extracting contents to prevent overwriting exisiting items."
}



#################################################### UNITY PLUGINS ##################################################


# Download the zipped Unity-gRPC plugins that are needed. 
Invoke-WebRequest -Uri "https://packages.grpc.io/archive/2019/12/a02d6b9be81cbadb60eed88b3b44498ba27bcba9-edd81ac6-e3d1-461a-a263-2b06ae913c3f/csharp/grpc_unity_package.2.26.0-dev.zip" -OutFile ".\Temp\unity_grpc.zip"
$UnityGrpcPluginsZipPath = Join-Path -Path $CurrentPath -ChildPath "\Temp\unity_grpc.zip"

$UnityPluginsExtractionDir = Join-Path -Path $CurrentPath -ChildPath "\Gemini-Unity\Assets"
$UnityPluginsDir = Join-Path -Path $UnityPluginsExtractionDir -ChildPath "\Plugins"

# Check if the Unity Plugins directory does not exist.
# If it already exists we do not want to extract the contents
# and overwrite the existing directory
if (-not (Test-Path -LiteralPath $UnityPluginsDir)) {
    Expand-Archive -LiteralPath $UnityGrpcPluginsZipPath -DestinationPath $UnityPluginsExtractionDir
}
else {
    "Plugins Directory '$UnityPluginsDir' already exists, extraction stopped to prevent overwriting."  
}


#################################################### CLEANUP ########################################################


# Cleanup, remove Temp download directory since this is not needed anymore
#Remove-Item $DownloadPath 
