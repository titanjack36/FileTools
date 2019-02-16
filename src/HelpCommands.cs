//-------------------------------HELP COMMANDS CLASS-------------------------------//
//@author TitanJack
//@project FileTools
//The Help Commands class is used to output information and documentation about 
//program commands to the console to assist the user

using System;

namespace FileTools {
    
    class HelpCommands {

        //Function Name: Display Help
        //@param command        The command to assist
        //       indent         Internal variable used to create spaces for indents
        //                      in the output
        //       tree           Specifies whether or not to output all commands in
        //                      a tree structure
        //@return               The string containg documentation for the specified
        //                      command
        public static string displayHelp(string command, string indent, bool tree) {
            if (indent == null) indent = "";
            else indent += "    ";
            string NL = "\n" + indent + "  ";
            string helpStr = "\n " + indent + ">";
            
            switch(command) {
                case "help":
                    helpStr += "help OR help tree" + NL + "(display availabe commands)";
                    break;
                case "exit":
                    helpStr += "exit" + NL + "exits the program";
                    break;
                case "filemanager":
                    helpStr += "fm OR filemanager" + NL + "(file manager tool)";
                    if (tree || indent == "") {
                        helpStr += 
                        displayHelp(command + " setdirectory", indent, tree) +
                        displayHelp(command + " getfiles", indent, tree) +
                        displayHelp(command + " editnames", indent, tree) +
                        displayHelp(command + " copyto", indent, tree) +
                        displayHelp(command + " moveto", indent, tree) +
                        displayHelp(command + " delete", indent, tree) +
                        displayHelp(command + " printselected", indent, tree) +
                        displayHelp(command + " clearselected", indent, tree) +
                        displayHelp(command + " printfiles", indent, tree);
                    }
                    break;
                case "filemanager setdirectory":
                    helpStr += "sd OR setdirectory OR dir [directory path]" + NL + "(set the path for the file directory)";
                    break;
                case "filemanager getfiles":
                    helpStr += "gf OR getfile OR getfiles OR sel OR select" + NL + "(get specific files from directory)";
                    if (tree || indent == "") 
                        helpStr += 
                        displayHelp(command + " all", indent, tree) +
                        displayHelp(command + " name", indent, tree) + 
                        displayHelp(command + " equals", indent, tree) + 
                        displayHelp(command + " contains", indent, tree) + 
                        displayHelp(command + " date", indent, tree) + 
                        displayHelp(command + " extension", indent, tree);
                    break;
                case "filemanager getfiles all":
                    helpStr += "all" + NL + "(copy all files)";
                    break;
                case "filemanager getfiles name":
                    helpStr += "name [file_name1] [file_name2] ..." + NL + "(gets file with name matching the specified name(s))";
                    break;
                case "filemanager getfiles equals":
                    helpStr += "eqls OR equals [key phrase] OR [index 1] [key phrase] OR [index 1] [index 2] [key phrase]" + NL + "(gets file that matches key phrase over specified interval)" + NL + "(Note: if only one index is entered, the end index will be the length of file name)";
                    break;
                case "filemanager getfiles contains":
                    helpStr += "ctns OR contains [key phrase] OR [index 1] [key phrase] OR [index 1] [index 2] [key phrase]" + NL + "(gets file that contains key phrase over specified interval)";
                    break;
                case "filemanager getfiles date":
                    helpStr += "dt OR date" + NL + "(gets file by matching date)";
                    if (tree || indent == "")
                        helpStr += 
                        displayHelp(command + " created", indent, tree) + 
                        displayHelp(command + " modified", indent, tree);
                    break;
                case "filemanager getfiles date created":
                case "filemanager getfiles date modified":
                    if (command == "filemanager getfiles date created") 
                        helpStr += "crtd OR created" + NL + "(compares file created date)";
                    else 
                        helpStr += "crtd OR modified" + NL + "(compares file modified date)";
                    if (tree || indent == "")
                        helpStr += 
                        displayHelp(command + " equals", indent, tree) +
                        displayHelp(command + " before", indent, tree) + 
                        displayHelp(command + " after", indent, tree);
                    break;
                case "filemanager getfiles date created equals":
                case "filemanager getfiles date modified equals":
                    helpStr += "eqls OR equals [date YYYY-MM-DD]" + NL + "(file date matches specified date)";
                    break;
                case "filemanager getfiles date created before":
                case "filemanager getfiles date modified before":
                    helpStr += "bfr OR before [date YYYY-MM-DD]" + NL + "(file date is earlier than specified date)";
                    break;
                case "filemanager getfiles date created after":
                case "filemanager getfiles date modified after":
                    helpStr += "aftr OR after [date YYYY-MM-DD]" + NL + "(file date is later than specified date)";
                    break;
                case "filemanager getfiles extension":
                    helpStr += "ext OR extension [extension name]" + NL + "(get file by extension type, example: .txt)";
                    break;
                case "filemanager editnames":
                    helpStr += "en OR editname OR editnames" + NL + "(edit names of the selected files)";
                    if (tree || indent == "") 
                        helpStr += 
                        displayHelp(command + " insert", indent, tree) +
                        displayHelp(command + " replace", indent, tree) + 
                        displayHelp(command + " replaceoccurrences", indent, tree) + 
                        displayHelp(command + " removeoccurrences", indent, tree) + 
                        displayHelp(command + " set", indent, tree);
                    break;
                case "filemanager editnames insert":
                    helpStr += "ins OR insert [index] [text]" + NL + "(insert text into file name at a specific index)";
                    break;
                case "filemanager editnames replace":
                    helpStr += "rpl OR replace [index 1] [text] OR [index 1] [index 2] [text]" + NL + "(replaces previous text over the specified interval with new specified text)" + NL + "(Note: if only one index is entered, the end index will be the length of file name)";
                    break;
                case "filemanager editnames replaceoccurrences":
                    helpStr += "rplocr OR replaceoccurrences [find text] [replace with text]" + NL + "(finds specified text in the file name and replace it with other text)";
                    break;
                case "filemanager editnames removeoccurrences":
                    helpStr += "remocr OR removeoccurrences [find text]" + NL + "(finds specified text and removes it from the file name)";
                    break;
                case "filemanager editnames set":
                    helpStr += "set [text]" + NL + "(changes file name to specified text)";
                    break;
                case "filemanager copyto":
                    helpStr += "ct OR copyto [destination directory]" + NL + "(Copy selected files to a new specified directory, skips duplicate files by default)" + NL + "(Additional OPTIONS: -r OR -replace)";
                    break;
                case "filemanager moveto":
                    helpStr += "mt OR moveto [destination directory]" + NL + "(Move selected files to a new specified directory, skips duplicate files by default)" + NL + "(Additional OPTIONS: -r OR -replace)";
                    break;
                case "filemanager delete":
                    helpStr += "dl OR delete" + NL + "(Removes all selected files permanently)";
                    break;
                case "filemanager printselected":
                    helpStr += "ps OR printselected" + NL + "(prints the names of selected files)";
                    break;
                case "filemanager clearselected":
                helpStr += "cs OR clearselected" + NL + "(clears previously selected files)";
                break;
                case "filemanager printfiles":
                    helpStr += "pf OR printfile OR printfiles" + NL + "(prints all files in current directory)";
                    break;
                default: helpStr = "Unknown command: " + command;
                    break;
            }
            return helpStr;
        }

        //Function Name: Get Full Command
        //@param command        A command inputed by the user
        //@return               The full name for the command if it matches one of
        //                      the short forms
        public static string getFullCommand(string command) {
            switch (command) {
                case "fm": return "filemanager";
                case "?": return "help";
                case "dir":
                case "sd": return "setdirectory";
                case "sel":
                case "select":
                case "getfile":
                case "gf": return "getfiles";
                case "editname":
                case "en": return "editnames";
                case "ps": return "printselected";
                case "cs": return "clearselected";
                case "printfile":
                case "pf": return "printfiles";
                case "eqls": return "equals";
                case "ctns": return "contains";
                case "dt": return "date";
                case "crtd": return "created";
                case "mdfd": return "modified";
                case "bfr": return "before";
                case "aftr": return "after";
                case "ext": return "extension";
                case "ins": return "insert";
                case "rpl": return "replace";
                case "rplocr": return "replaceoccurrences";
                case "remocr": return "removeoccurrences";
                case "-r": return "-replace";
                case "ct": return "copyto";
                case "mt": return "moveto";
                case "dl": return "delete";
                default: return command;
            }
        }
    }
}