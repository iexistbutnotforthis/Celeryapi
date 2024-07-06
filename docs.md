## API Docs

##### Add the API DLL as a reference, after that add it by doing `using CeleryAPI;`
##### Then u can inject, to inject do  `ExploitApi.Inject();`
##### To execute do `ExploitApi.Execute(textBox1.Text, useCustomUnc: false);`
The textBox1.Text is just a string, also in order to use custom unc u gotta do this at the start of your code `ExploitApi.ChangeExecuterIdentityName();`

##### To close Roblox u can do `ExploitApi.Close()`
##### To check if its injected, u can do `ExploitApi.CheckInjectionStats()`, it will return true if its injected, and false if its not


###If you dont understand something from here, ask in our [Discord server](https://discord.gg/u2YWpA8y7U "Discord server")
