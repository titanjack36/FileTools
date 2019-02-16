//------------------------------------FILE TOOLS------------------------------------//
//@author TitanJack
//@version 1.0 (2019-02-14)
//File Tools is a command console program meant to make file manipulation easier.
//The program is most useful in instances where large amounts of files may make it
//difficult to perform operations on them.

using System;
using System.Collections.Generic;

namespace FileTools
{
    class ConsoleProgram
    {
        static void Main(string[] args)
        {
            bool run = true;
            FileManager fm = new FileManager();
            Console.WriteLine("FileTools Program Console V1.0\nType \"help\" for a list of commands or \"help tree\" for detailed usage of commands");
            while (run) {
                Console.Write("\n>");
                string input = Console.ReadLine();
                string[] attribs = splitInputToArray(input);
                if (attribs.Length >= 1) {
                    attribs[0] = HelpCommands.getFullCommand(attribs[0]);
                    if (attribs[0] == "filemanager") {
                        if (attribs.Length >= 2) {
                            
                            attribs[1] = HelpCommands.getFullCommand(attribs[1]);
                            if (attribs[1] == "setdirectory") {
                                if (attribs.Length == 3) fm.setDirectory(attribs[2]);
                                else error(attribs[0] + " " + attribs[1], "attribamt");

                            } else if (attribs[1] == "getfiles") {
                                if (attribs.Length >= 3) fm.getFiles(getSubArray(attribs, 2));
                                else error(attribs[0] + " " + attribs[1], "attribamt");

                            } else if (attribs[1] == "editnames") {
                                if (attribs.Length >= 3) fm.editNames(getSubArray(attribs, 2));
                                else error(attribs[0] + " " + attribs[1], "attribamt");

                            } else if (attribs[1] == "copyto") {
                                if (attribs.Length >= 3) fm.copyOrMoveFilesTo(attribs[1], getSubArray(attribs, 2));
                                else error(attribs[0] + " " + attribs[1], "attribamt");

                            } else if (attribs[1] == "moveto") {
                                if (attribs.Length >= 3) fm.copyOrMoveFilesTo(attribs[1], getSubArray(attribs, 2));
                                else error(attribs[0] + " " + attribs[1], "attribamt");
                            
                            } else if (attribs[1] == "delete") {
                                if (attribs.Length == 2) fm.delete();
                                else error(attribs[0] + " " + attribs[1], "attribamt");
                            } else if (attribs[1] == "printselected") {
                                if (attribs.Length == 2) fm.printSelectedFiles();
                                else error(attribs[0] + " " + attribs[1], "attribamt");

                            } else if (attribs[1] == "clearselected") {
                                if (attribs.Length == 2) fm.clearSelection();
                                else error(attribs[0] + " " + attribs[1], "attribamt");

                            } else if (attribs[1] == "printfiles") {
                                if (attribs.Length == 2) fm.printAllFilesInDirectory();
                                else error(attribs[0] + " " + attribs[1], "attribamt");

                            } else error(attribs[0], "attrib");
                        } else error(attribs[0], "attrib");

                    } else if (attribs[0] == "help") {
                        bool tree = attribs.Length == 2 && attribs[1] == "tree";
                        Console.Write("Showing all available commands: ");
                        Console.Write(HelpCommands.displayHelp("filemanager", null, tree) +
                                    HelpCommands.displayHelp("help", null, tree) +
                                    HelpCommands.displayHelp("exit", null, tree) + "\n");

                    } else if (attribs[0] == "exit") {
                        run = false;
                    } else error(null, "unknown command, type \"help\" for a list of commands");
                }
            }
        }

        //Function Name: Get Sub Array
        //@param arr        The array to perform operations on
        //       begin      Starting index
        //       end        Ending index
        //@return           An array composed of elements from starting to ending index
        //                  of the original array
        private static string[] getSubArray(string[] arr, int begin, int end) {
            if (begin < end && begin >= 0 && end <= arr.Length) {
                string[] subarr = new string[end - begin];
                for (int i = begin; i < end; i++) {
                    subarr[i - begin] = arr[i];
                }
                return subarr;
            } else {
                error(null, "Invalid indices for begin and end");
                return null;
            }
        }

        //Function Name: Get Sub Array
        //@param arr        The array to perform operations on
        //       begin      Starting index
        //@return           The subarray where the ending index is the last index of 
        //                  of the original array
        private static string[] getSubArray(string[] arr, int begin) {
            return getSubArray(arr, begin, arr.Length);
        }

        //Function Name: Split Input To Array
        //@param str        The user input string
        //@return           The input string split by spaces into an array
        //                  (The use of quotation marks prevents the enclosed string 
        //                  from being split)
        private static string[] splitInputToArray(string str) {
            List<string> list = new List<string>();
            while (str.Contains(" ") ||  str.Contains("\"")) {
                int spaceIndex = str.IndexOf(" "), quoteIndex = str.IndexOf("\"");
                if (spaceIndex == 0) str = str.Substring(1);
                else {
                    if (quoteIndex == -1 || (spaceIndex != -1 && spaceIndex < quoteIndex)) {
                        list.Add(str.Substring(0, spaceIndex));
                        str = str.Substring(spaceIndex + 1);
                    } else {
                        if (quoteIndex != -1) {
                            string line = str.Substring(0, quoteIndex);
                            str = str.Substring(quoteIndex + 1);
                            line += str.Substring(0, str.IndexOf("\""));
                            str = str.Substring(str.IndexOf("\"") + 1);
                            list.Add(line);
                        }
                    }
                }
            }
            if (str.Length != 0) {
                list.Add(str);
            }
            return list.ToArray();
        }

        //Function: Error
        //@param command        The command which produced the error
        //       reason         The cause for the error
        //Prints out an error message for when the user inputs an incorrect
        //command and displays the correct way to execute the command
        private static void error(string command, string reason) {
            if (reason != null) {
                String errorMsg = reason;
                switch (reason) {
                    case "attribamt":
                        errorMsg = "invalid amount of attributes";
                        break;
                    case "attrib":
                        errorMsg = "invalid attributes";
                        break;
                    case "attribnum":
                        errorMsg = "attributes should be in number form";
                        break;
                }
                Console.Error.WriteLine("Error: " + errorMsg);
            }
            if (command != null) {
                Console.Write("Usage: ");
                Console.Write(HelpCommands.displayHelp(command, null, false) + "\n");
            }   
        }
    }
}
