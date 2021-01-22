mkdir temp
	
Invoke-WebRequest -Uri "https://packages.grpc.io/archive/2019/12/a02d6b9be81cbadb60eed88b3b44498ba27bcba9-edd81ac6-e3d1-461a-a263-2b06ae913c3f/protoc/grpc-protoc_windows_x64-1.26.0-dev.zip" -OutFile ".\temp\grpc.zip"

$CurrentPath = pwd

$GrpcPath = Join-Path -Path $CurrentPath -ChildPath "\temp\grpc.zip" 

echo $GrpcPath

# TODO: Swap \out with the real path below
#$ExtractionDirectory = Join-Path -Path $CurrentPath -ChildPath "\API\Protobuf\Plugins"
$ExtractionDirectory = Join-Path -Path $CurrentPath -ChildPath "\out"

if (-not (Test-Path -LiteralPath $ExtractionDirectory)) {
    try {
        "The directory '$ExtractionDirectory' does not exist, this needs to be created."
        New-Item -Path $ExtractionDirectory -ItemType Directory -ErrorAction Stop | Out-Null #-Force
    }
    catch {
        Write-Error -Message "Unable to create directory '$ExtractionDirectory'. Error was: $_" -ErrorAction Stop 
    }
    "Successfully created directory '$ExtractionDirectory'."
    "Extracting contents from '$GrpcPath'."
    Expand-Archive -LiteralPath $GrpcPath -DestinationPath $ExtractionDirectory 

}
else {
    "Directory '$ExtractionDirectory' already exists, extracting contents from '$GrpcPath' to '$ExtractionDirectory'."
    Expand-Archive -LiteralPath $GrpcPath -DestinationPath $ExtractionDirectory 
}

