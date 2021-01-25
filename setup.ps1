function CheckAndCreateDirectory {
    param (
        $DirectoryPath
    )

    # Create a new directory with $DirectoryPath since it does not exist.
    if (-not (Test-Path -LiteralPath $DirectoryPath)) {
        try {
            "The directory '$DirectoryPath' does not exist, this needs to be created."
            New-Item -Path $DirectoryPath -ItemType Directory -ErrorAction Stop | Out-Null #-Force
        }
        catch {
            Write-Error -Message "Unable to create directory '$DirectoryPath'. Error was: $_" -ErrorAction Stop 
        }
        "Successfully created directory '$DirectoryPath'."
    }
    else {
        # The directory already exists
        "Directory '$DirectoryPath' already exists"
    }
}

# Creating directories needed for extracting Protobuf and gRPC dependencies.
$CurrentPath = Get-Location
$DownloadPath = Join-Path -Path $CurrentPath -ChildPath "\Temp"
CheckAndCreateDirectory($DownloadPath)

# Download the zipped gRPC binaries that are needed. 
Invoke-WebRequest -Uri "https://packages.grpc.io/archive/2019/12/a02d6b9be81cbadb60eed88b3b44498ba27bcba9-edd81ac6-e3d1-461a-a263-2b06ae913c3f/protoc/grpc-protoc_windows_x64-1.26.0-dev.zip" -OutFile ".\Temp\grpc.zip"

# The path to the file to be unzipped and extracted
$GrpcZipPath = Join-Path -Path $CurrentPath -ChildPath "\Temp\grpc.zip" 

# The path to where the contents of the zip file should be extracted
$ExtractionDirectory = Join-Path -Path $CurrentPath -ChildPath "\API\Protobuf\Plugins"

# Check whether the Plugins direcotry already exists, if not create it before extracting the zip file.
Expand-Archive -LiteralPath $GrpcZipPath -DestinationPath $ExtractionDirectory 
