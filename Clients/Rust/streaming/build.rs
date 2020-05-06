fn main() -> Result<(), Box<dyn std::error::Error>> {
    tonic_build::compile_protos("../../../Protobuf/ProtoFiles/sensordata/sensordata.proto")?;
    Ok(())
}