# ISSServerAgent
ISS服务器托管

To give Insurgency:Sandstorm Server Some Commands When It Starts a Round to Override the Config Set by the Ruleset.

当回合开始时给叛乱：沙漠风暴服务端一些指令来覆盖被规则集所设定的配置



# ATTENTION
.NET Core 3.1 is required to run this program.

要运行该程序需要.NET Core 3.1.

https://dotnet.microsoft.com/download/dotnet-core/3.1



# How to use
1. Put program at the root of server. (Where the `InsurgencyServer.exe` is located)

2. Create two txt files and a cmd file in same location. (Example: *a*.txt, *b*.txt and *c*.cmd)

3. Writing your server config in  *a*.txt . (Example content as "*`Precinct?Scenario=Scenario_Precinct_Checkpoint_Security?MaxPlayers=8 -Port=37102 -QueryPort=37131 -AdminList=Admins -MapCycle=Mapcycle -hostname="yourname"`*", you should remove the outside "", but DO NOT REMOVE which follows "`-hostname=`".)

4. Writing your commands in  *b*.txt , use NewLine to spilt each commands. They're as same as the rcon commands.

5. Writing the  *c*.cmd  as following example: "`ISSServerProt.exe --argsFile `*`a`*`.txt --rconPort `*`port`*` --rconPassword `*`password`*` --name `*`Normal`*` --CommandFile `*`b`*`.txt`".

6. Run the  *c*.cmd , you should join the server after it started at least 45 secs. But if it starts so slowly that the program's connect request is sended earlier than server could accept them, the commands will not be executed correctly.


### 如何使用
1. 将程序放在服务器根目录下。 (`InsurgencyServer.exe`所在的位置)

2. 在相同位置创建一个cmd文件和两个txt文件。(例如 *a*.txt ， *b*.txt 和 *c*.cmd )

3. 在 *a*.txt 写下你的服务器配置。(示例内容："*`Precinct?Scenario=Scenario_Precinct_Checkpoint_Security?MaxPlayers=8 -Port=37102 -QueryPort=37131 -AdminList=Admins -MapCycle=Mapcycle -hostname="yourname"`*"，你应该删去外侧的 "" ，但不要删除跟着"`-hostname=`"的。)

4. 在 *b*.txt 写下你的指令，用换行符分割各个指令。它们和rcon指令一样。

5. 依照以下示例编写 *c*.cmd ： "`ISSServerProt.exe --argsFile `*`a`*`.txt --rconPort `*`port`*` --rconPassword `*`password`*` --name `*`Normal`*` --CommandFile `*`b`*`.txt`" 。

6. 运行*c*.cmd，你应该在服务器启动45秒后再加入。但如果它启动的很慢，以至于程序的连接请求在服务端能接受之前就被发送，指令将不会被正确执行。
