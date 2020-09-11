using System;
using System.Diagnostics;

namespace Oltman_Lab0x01
{
    class Program
    {
        static void Main(string[] args)
        {
            var sandbox = new SearchSandbox();
            if (!sandbox.VerificationTests())
            {
                Console.WriteLine("Verification Tests have failed. Quitting Program...");
                return;
            }
            sandbox.RunTimeTests();
        }
    }
}
