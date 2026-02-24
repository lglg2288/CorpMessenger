using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Models
{
    public static class CurrentGrpcChannel
    {
        public static GrpcChannel channel = GrpcChannel.ForAddress("http://neogus.ru:5203", new GrpcChannelOptions { });
    }
}
