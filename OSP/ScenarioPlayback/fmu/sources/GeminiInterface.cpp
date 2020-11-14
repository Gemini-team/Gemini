#include <cmath>
#include <cstring>
#include <cppfmu_cs.hpp>
#include "GeminiInterface-fmu-uuid.h"
#include <stdio.h>
#include <stdlib.h>
#include <iostream>
#include <boost/asio.hpp>
#include <boost/array.hpp>
#include <boost/algorithm/string.hpp> 
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/xml_parser.hpp>
#include <boost/property_tree/json_parser.hpp>
#include <boost/foreach.hpp>
#include <boost/range/algorithm.hpp>
#include <filesystem>
#include <iostream>
#include <memory>
#include <string>

#include <grpcpp/grpcpp.h>
#include "GeminiOSPInterface.pb.h"

#ifdef BAZEL_BUILD
#include "GeminiOSPInterface.grpc.pb.h"
#else
#include "GeminiOSPInterface.grpc.pb.h"
#endif


using grpc::Channel;
using grpc::ClientContext;
using grpc::Status;
using GeminiOSPInterface::Simulation;
using GeminiOSPInterface::Pose;
using GeminiOSPInterface::StepRequest;
using GeminiOSPInterface::StepResponse;
using GeminiOSPInterface::SetStartTimeRequest;
using GeminiOSPInterface::SetStartTimeResponse;

using namespace boost::asio;
namespace pt = boost::property_tree;
namespace fs = std::filesystem;
using ip::tcp;
using std::vector;
using std::string;
using std::cout;
using std::endl;
using std::to_string;


string getDllPath();
string sec2TimeOfDay(double sec);

class OSPClient {

public:
    OSPClient(std::shared_ptr<Channel> channel)
        : stub_(Simulation::NewStub(channel)) {}

    // Assembles the client's payload, sends it and presents the response back
    // from the server
    bool DoStep(StepRequest* request, StepResponse* response) {
        
        // Context for the client. It could be used to convey extra information to
        // the server and/or tweak certain RPC behaviors
        ClientContext context;
        
        // The actual RPC.
        Status status = stub_->DoStep(&context, *request, response);

        // Act upon the status  
        if (status.ok()) {
            return true;
        }
        else
        {
            std::cout << status.error_code() << ": " << status.error_message()
                << std::endl;
            return false;
            
        }

        
    }

    bool SetStartTime(SetStartTimeRequest* request, SetStartTimeResponse* response) {

        // Context for the client. It could be used to convey extra information to
        // the server and/or tweak certain RPC behaviors
        ClientContext context;

        // The actual RPC.
        Status status = stub_->SetStartTime(&context, *request, response);

        // Act upon the status  
        if (status.ok()) {
            return true;
        }
        else
        {
            std::cout << status.error_code() << ": " << status.error_message()
                << std::endl;
            return false;

        }


    }

    
private:
    std::unique_ptr<Simulation::Stub> stub_;
};



class GeminiInterface: public cppfmu::SlaveInstance
{
public:
    GeminiInterface(const cppfmu::Memory& memory)
    {
        GeminiInterface::Reset();
        //this->client = cppfmu::AllocateUnique<OSPClient>(memory, grpc::CreateChannel(
        //    "localhost:12346", grpc::InsecureChannelCredentials()));
    }

    void Terminate() {
        std::cout << "Terminating" << std::endl;
        //client.release();
        std::cout << "Post release" << std::endl;
    }

    void SetupExperiment(
        cppfmu::FMIBoolean toleranceDefined,
        cppfmu::FMIReal tolerance,
        cppfmu::FMIReal tStart,
        cppfmu::FMIBoolean stopTimeDefined,
        cppfmu::FMIReal tStop) override
    {
        //Parse XML file
        auto dll_path = fs::path(getDllPath());
        auto xml_path = dll_path.parent_path().parent_path().parent_path() / "modelDescription.xml";
        pt::ptree tree;
        pt::read_xml(xml_path.string(), tree);

        BOOST_FOREACH(pt::ptree::value_type & v, tree.get_child("fmiModelDescription.ModelVariables")) {
            if (v.first == "ScalarVariable") {
                if (v.second.get_child("<xmlattr>.name").data() == "server_ip") {
                    server_ip = v.second.get_child("String.<xmlattr>.start").data();
                    ip_vr = std::stoi(v.second.get_child("<xmlattr>.valueReference").data());
                }
                else if (v.second.get_child("<xmlattr>.name").data() == "server_port") {
                    server_port = v.second.get_child("String.<xmlattr>.start").data();
                    port_vr = std::stoi(v.second.get_child("<xmlattr>.valueReference").data());
                }
                else if (v.second.get_child("<xmlattr>.name").data() == "start_time") {
                    start_time = v.second.get_child("String.<xmlattr>.start").data();
                    start_time_vr = std::stoi(v.second.get_child("<xmlattr>.valueReference").data());
                }
                else if (v.second.get_child("<xmlattr>.name").data() == "number_of_vessels") {
                    n_vessels = std::stoi(v.second.get_child("Integer.<xmlattr>.start").data());
                    n_vessels_vr = std::stoi(v.second.get_child("<xmlattr>.valueReference").data());
                }
                else if (v.second.get_child("<xmlattr>.causality").data() == "input"){
                    input_names.push_back(v.second.get_child("<xmlattr>.name").data());
                    input_vr.push_back(std::stoi(v.second.get_child("<xmlattr>.valueReference").data()));

                }
                else if (v.second.get_child("<xmlattr>.causality").data() == "output") {
                    output_names.push_back(v.second.get_child("<xmlattr>.name").data());
                    output_vr.push_back(std::stoi(v.second.get_child("<xmlattr>.valueReference").data()));
                }
            }
        }
        n_input = input_vr.size();
        n_output = output_vr.size();

        for (int i = 0; i < n_input; i++) {
            from_fmi.push_back(0.0);
        }

        for (int i = 0; i < n_output; i++) {
            from_server.push_back(0.0);
        }

		//Create gRPC Client
		cout << "Server port =  " << server_port << endl;
        cout << "Server IP =  " << server_ip << endl;
		std::string target_str = server_ip + ":" + server_port;
        this->client = std::make_unique<OSPClient>(grpc::CreateChannel(target_str, grpc::InsecureChannelCredentials()));
        
        //Set start time
        SetStartTimeRequest* request = new SetStartTimeRequest;
        request->set_time(start_time);
        SetStartTimeResponse* response = new SetStartTimeResponse;
        client->SetStartTime(request, response);
        if (!(response->success())) throw std::logic_error("Gemini Server reports unsucsessful RPC call when setting start time");


        cout << "Signals from OSP to Unity:" << endl;
        for (std::vector<string>::const_iterator i = input_names.begin(); i != input_names.end(); ++i)
            cout << *i << endl;
        cout << "Signals from Unity to OSP:" << endl;
        for (std::vector<string>::const_iterator i = output_names.begin(); i != output_names.end(); ++i)
            cout << *i << endl;
		
    }

    void Reset() override
    {
        for (int i = 0; i < n_input; i++) {
            from_fmi[i] = 0.0;
        }

        for (int i = 0; i < n_output; i++) {
            from_server[i] = 0.0;
        }

    }

    void SetString(
        const cppfmu::FMIValueReference vr[],
        std::size_t nvr,
        const cppfmu::FMIString value[]) override
    {
        cout << "Cannot modify server ip and port or start_time!" << endl;

    }

    void SetInteger(
        const cppfmu::FMIValueReference vr[],
        std::size_t nvr,
        const cppfmu::FMIInteger value[]) override
    {
        cout << "Cannot modify number of vessels dynamically!" << endl;

    }

    void SetReal(
        const cppfmu::FMIValueReference vr[],
        std::size_t nvr,
        const cppfmu::FMIReal value[]) override
    {
        
        __int64 index;
        for (std::size_t i = 0; i < nvr; ++i) {
            index = std::distance(input_vr.begin(), std::find(input_vr.begin(), input_vr.end(), vr[i]));
            from_fmi[index] = value[i];
        }
    }

    void GetString(
        const cppfmu::FMIValueReference vr[],
        std::size_t nvr,
        cppfmu::FMIString value[]) const override
    {
        for (std::size_t i = 0; i < nvr; ++i) {
            if (vr[i] == ip_vr)
                value[i] = server_ip.c_str();
            else if (vr[i] == port_vr)
                value[i] = server_port.c_str();
            else if (vr[i] == start_time_vr)
                value[i] = start_time.c_str();
            else
                throw std::logic_error("Invalid value reference when getting string");
        }
    }

    void GetReal(
        const cppfmu::FMIValueReference vr[],
        std::size_t nvr,
        cppfmu::FMIReal value[]) const override
    {

        __int64 index_out;
        __int64 index_in;
        for (std::size_t i = 0; i < nvr; ++i) {
            index_out = std::distance(output_vr.begin(), std::find(output_vr.begin(), output_vr.end(), vr[i]));
            index_in = std::distance(input_vr.begin(), std::find(input_vr.begin(), input_vr.end(), vr[i]));
            if (index_out < (int) n_output) {
                value[i] = from_server[index_out];
            }
            else if (index_in < (int) n_input) {
                value[i] = from_fmi[index_in];      
            }
            else {
                value[i] = 0.0;
            }
        }
    }

    void GetInteger(
        const cppfmu::FMIValueReference vr[],
        std::size_t nvr,
        cppfmu::FMIInteger value[]) const override
    {
        for (std::size_t i = 0; i < nvr; ++i) {
            if (vr[i] == n_vessels_vr)
                value[i] = n_vessels;
            else
                throw std::logic_error("Invalid value reference when getting integer");
        }
    }

    bool DoStep(
        cppfmu::FMIReal currentCommunicationPoint,
        cppfmu::FMIReal dt,
        cppfmu::FMIBoolean /*newStep*/,
        cppfmu::FMIReal& /*endOfStep*/) override
    {
     
     StepRequest* request = new StepRequest;

     for (int i = 0; i < n_vessels; i++) {
         int idx = i * 3;
         Pose* pose = request->add_vesselposes();
         pose->set_north(from_fmi[idx]);
         pose->set_east(from_fmi[idx+1]);
         pose->set_heading(from_fmi[idx+2]);
     }

     request->set_stepsize(dt);
     request->set_time(currentCommunicationPoint);
     StepResponse* response = new StepResponse;
     client->DoStep(request, response);
     if (!(response->success())) throw std::logic_error("Gemini Server reports unsucsessful RPC call for DoStep");
     return true;
    }

private:
    //cppfmu::UniquePtr<OSPClient> client;
    std::unique_ptr<OSPClient> client;
    vector<double> from_server;
    vector<double> from_fmi;
    string server_ip;
    string server_port;
    int ip_vr;
    int port_vr;
    int n_vessels;
    int n_vessels_vr;
    string start_time;
    int start_time_vr;
    vector<string> input_names;
    vector<string> output_names;
    vector<cppfmu::FMIValueReference> input_vr;
    vector<cppfmu::FMIValueReference> output_vr;
    size_t n_input;
    size_t n_output;
};


cppfmu::UniquePtr<cppfmu::SlaveInstance> CppfmuInstantiateSlave(
    cppfmu::FMIString  /*instanceName*/,
    cppfmu::FMIString  fmuGUID,
    cppfmu::FMIString  /*fmuResourceLocation*/,
    cppfmu::FMIString  /*mimeType*/,
    cppfmu::FMIReal    /*timeout*/,
    cppfmu::FMIBoolean /*visible*/,
    cppfmu::FMIBoolean /*interactive*/,
    cppfmu::Memory memory,
    cppfmu::Logger /*logger*/)
{
    if (std::strcmp(fmuGUID, FMU_UUID) != 0) {
        throw std::runtime_error("FMU GUID mismatch");
    }
    return cppfmu::AllocateUnique<GeminiInterface>(memory, memory);
}



string getDllPath() {

    char path[MAX_PATH];
    HMODULE hm = NULL;

    if (GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS |
        GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
        (LPCSTR)&getDllPath, &hm) == 0)
    {
        int ret = GetLastError();
        fprintf(stderr, "GetModuleHandle failed, error = %d\n", ret);
        // Return or however you want to handle an error.
    }
    if (GetModuleFileName(hm, path, sizeof(path)) == 0)
    {
        int ret = GetLastError();
        fprintf(stderr, "GetModuleFileName failed, error = %d\n", ret);
        // Return or however you want to handle an error.
    }

    // The path variable should now contain the full filepath for this DLL.
    return string(path);
}

string sec2TimeOfDay(double sec) {
    vector<string> strs;
    double rem;
    int hrs = floor(sec / 3600);
    rem = sec - hrs * 3600;
    int mins = floor(rem / 60);
    rem -= mins * 60;
    return to_string(hrs) + ":" + to_string(mins) + ":" + to_string(rem);
}
