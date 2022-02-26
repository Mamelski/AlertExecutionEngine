## How to run

### 1. Console

The solution is written in C#.

To run it you need to have dotnet 6 installed, you can download it
from [here](https://dotnet.microsoft.com/en-us/download).

**Before running the project make sure that alerts-api is running.**
You can run it from docker:

`docker run --name alerts_server -d -p 9001:9001 quay.io/chronosphereiotest/interview-alerts-engine:latest`

After that go to `AlertExecutionEngine` folder (inside unpacked .zip file) and run:

`dotnet build`

When build is successful run:

`dotnet run --project AlertExecutionEngine.Cli\AlertExecutionEngine.Cli.csproj`

Alert execution engine is up and running.

### 2. Docker compose (easiest)

Another option to run solution is to use docker-compose.

Go to go to `AlertExecutionEngine` folder and type in console:

`docker-compose up -d`

Two services (alerts-server and alerts-monitor) should be up and running

### 3. IDE

Open `AlertExecutionEngine\AlertExecutionEngine.sln` in your favourite IDE (JetBrains Rider :)) and run it from there.

## Comments

- Application passes validation which is nice
- Application can be configured in file `AlertExecutionEngine\AlertExecutionEngine.Cli\appsettings.json` or when
  using `docker-compose`
  in `AlertExecutionEngine\docker-compose.yaml` file via environment section for `alerts-monitor` service. When
  using `appsettings.json` project needs to be rebuilt
  (or you can copy `appsettings.json` file to output by yourself, or edit the one in output right away).
- Application prints to console "." every second and prints alerts when they are executed. This made it easier for me to
  test and debug solution.
- I did not implemented any logging in the app (except those Console writes). In non-interview projects there should be
  some meaningful structured logging.
- I did not implement any exception/error handling. In non-interview project there may be need for exception handling,
  maybe some retries, error codes, etc.
- I did not write any tests. I am not a fan of TDD, but still some Integration/Acceptance tests should be written to
  enable future changes and refactorings without fear of breaking something.
- When I started writing this solution I implemented persistence layer to store alerts, but I decided it is not needed
  and I keep everything in memory. In more complicated solutions there is usually need for persistence.
- I hope that solution is clear and easy to read. I added a bit more comments than normally, most of the stuff should be
  clear from naming and code itself.
- Some of the patterns (DI registration, interfaces, configuration, separate models for and external service) may be a
  little overkill for this project but they are standard in non-interview projects and I am used to them (and wanted to
  show you that).
- In general I have made some assumptions, naming decisions and design decisions that could be different, depending of
  broader team/project/product landscape, conventions and practices. In non-interview projects I am more aware and
  cautious with those things.

This was a fun task, I hope you will enjoy my solution, please let know in case of any problems/question and I can't
wait for you feedback :)

Jakub Mamelski (jakub.mamelski@outlook.com)