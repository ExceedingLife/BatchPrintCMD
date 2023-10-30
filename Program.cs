using Microsoft.Extensions.Http;

// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

Console.WriteLine("Enter 1 for SYSTEM Manuals or 2 for INSTALLATION Manuals");

bool validInput = false;
bool stopLoadingThread = false;
var initialStart = Console.ReadLine();

while (!validInput)
{   

    if (!string.IsNullOrEmpty(initialStart))
    {
        if (int.TryParse(initialStart, out int initialRequest))
        {
            if (initialRequest == 1)
            {
                validInput = true;
            }
            else if (initialRequest == 2)
            {
                validInput = true;
            }
            else
            {
                Console.WriteLine("Incorrect Value, Please try again");
            }
        }
        else
        {
            Console.WriteLine("Incorrect Value, Please try again");
        }
    }
    else
    {
        Console.WriteLine("Incorrect Value, Please try again");
    }
}

if (validInput)
{
    Console.WriteLine("Please Enter a Project Number");
    

    var project = Console.ReadLine();
    Thread tAPI = null;
    Thread tLoading = new Thread(ShowLoadingSymbol);


    HttpClient httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(360)
    };

    Console.WriteLine($"Starting PDF Generation of Project #: {project}");


    try
    {
        if (initialStart.Equals("1"))
        {
            tLoading.Start();

            tAPI = new Thread(() =>
            {
                // Run API Calls in a Separate Thread
                Task.Run(async () =>
                {
                    #region Before Method Created
                    //using (var httpResponse = await httpClient.GetAsync($"http://10.20.55.38:5025/System/GetManualsByProject?projectNumber={project}"))
                    //{
                    //    Console.WriteLine("Inside Request");

                    //    if (httpResponse.IsSuccessStatusCode)
                    //    {
                    //        Console.WriteLine(await httpResponse.Content.ReadAsStringAsync());
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine($"Error: {httpResponse.StatusCode} {httpResponse.RequestMessage}");
                    //    }
                    //} 
                    #endregion
                    await GetSystemManualsByProjectAsync(project, httpClient);
                    //Thread.Sleep(3000);
                    stopLoadingThread = true;

                }).Wait(); // Wait for Task to Complete
            });

            tAPI.Start();
            tAPI.Join();

        }
        else if (initialStart.Equals("2"))
        {
            tLoading.Start();

            tAPI = new Thread(() =>
            {
                // Run API Calls in a Separate Thread
                Task.Run(async () =>
                {

                    await GetInstallManualsByProjectAsync(project, httpClient);
                    //Thread.Sleep(3000);
                    stopLoadingThread = true;

                }).Wait(); // Wait for Task to Complete
            });

            tAPI.Start();
            tAPI.Join();
        }

        //Console.CursorVisible = true;

        // Show "Still running" until API thread is finished.
        //while (tAPI.IsAlive)
        //{
        //    Console.WriteLine("Application is running");
        //    Thread.Sleep(1000);
        //}
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}




static async Task<string> GetSystemManualsByProjectAsync(string project, HttpClient httpClient)
{
    using (var httpResponse = await httpClient.GetAsync($"http://10.20.55.38:5025/System/GetManualsByProject?projectNumber={project}"))
    {
        //Console.WriteLine("Inside Request");

        if (httpResponse.IsSuccessStatusCode)
        {
            Console.WriteLine(await httpResponse.Content.ReadAsStringAsync());
        }
        else
        {
            Console.WriteLine($"Error: {httpResponse.StatusCode} {httpResponse.RequestMessage}");
        }

        return await httpResponse.Content.ReadAsStringAsync();
    }
}

static async Task<string> GetInstallManualsByProjectAsync(string project, HttpClient httpClient)
{
    using (var httpResponse = await httpClient.GetAsync($"http://10.20.55.38:5025/Install/GetInstallByProject?projectNumber={project}"))
    //using (var httpResponse = await httpClient.GetAsync($@"http://localhost:5081/Install/GetInstallByProject?projectNumber={project}"))
    {
        //Console.WriteLine("Inside Request");

        if (httpResponse.IsSuccessStatusCode)
        {
            Console.WriteLine();
            Console.WriteLine(await httpResponse.Content.ReadAsStringAsync());
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine($"Error: {httpResponse.StatusCode} {httpResponse.RequestMessage}");
        }

        return await httpResponse.Content.ReadAsStringAsync();
    }
}


void ShowLoadingSymbol()
{
    // Define the loading symbols
    char[] loadingSymbols = { '|', '/', '-', '\\' };
    int index = 0;

    while (!stopLoadingThread)
    {
        // Move the cursor to the beginning of the line and overwrite the current line with the loading symbol
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write($"Loading {loadingSymbols[index]}");

        // Increment the loading symbol index
        index = (index + 1) % loadingSymbols.Length;

        // Sleep for a short time to create the loading animation
        Thread.Sleep(100);
    }

    // Clear the loading symbol line when the loading is done
    Console.SetCursorPosition(0, Console.CursorTop);
    Console.Write(new string(' ', Console.BufferWidth - 1));
}