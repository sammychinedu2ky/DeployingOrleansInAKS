var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");
//var orleans = builder.AddOrleans("orleans-cluster")
//    .WithClustering(redis);

var api = builder.AddProject<Projects.API>("api")
    .WithReplicas(3)
    .WithReference(redis)
    .WithEndpoint(name: "ORLEANS-SILO-DASHBOARD", port: 977, env: "ORLEANS-SILO-DASHBOARD", scheme: "http");

builder.Build().Run();




