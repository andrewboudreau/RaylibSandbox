import { dotnet } from './_framework/dotnet.js'

await dotnet
    .withDebugging(1)
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

dotnet.instance.Module['canvas'] = document.getElementById('canvas');

console.log("starting dotnet.run");
const loading_div = document.getElementById('spinner');
loading_div.remove();

await dotnet.run();