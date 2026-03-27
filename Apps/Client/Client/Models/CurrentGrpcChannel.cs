using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Client.Models
{
    public static class CurrentGrpcChannel
    {
        public static GrpcChannel channel = GrpcChannel.ForAddress("http://192.168.0.22:5203", new GrpcChannelOptions{});//10.0.2.2 - lookback на эмуляторе
    }
}
