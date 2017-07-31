using RawInputDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleApplication
{
    class Program
    {
        private static RawInput rawInput;

        private static MouseEventArgs mouseEvent;

        static void Main(string[] args)
        {
            rawInput = new RawInput();


            rawInput.MouseEvent += OnMouseEvent;

            Console.ReadKey();
        }

        private static void OnMouseEvent(object sender, MouseEventArgs e)
        {
            mouseEvent = e;

            UpdateConsole();
        }

        private static void UpdateConsole()
        {
            Console.Clear();
            Console.WriteLine($"Mouse: ");
            Console.WriteLine($"\tFlags:    {mouseEvent?.Flags}");
            Console.WriteLine($"\tAllKeys:  {mouseEvent?.AllKeys}");
            Console.WriteLine($"\tKey:      {mouseEvent?.Key}");
        }
    }
}
