//--------------------------------FILE MANAGER CLASS--------------------------------//
//@author TitanJack
//@project FileTools
//The file manager performs file manipulation by moving, deleting or re-naming files
//based on the specifications given by the user in the command console
//TODO: create function for moving and deleting files

using System;
using System.Collections.Generic;
using System.IO;

namespace FileTools {

    class FileManager {
        
        private string directory;
        private List<string> selectedFiles;

        public FileManager() {
            directory = null;
            selectedFiles = new List<string>();
        }

        //------------------------------CORE FUNCTIONS------------------------------//
        //FUNCTION LIST:
        //public void getFiles(string[] attribs)
        //public void editNames(string [] attribs)
        //public void copyOrMoveFilesTo(string copyOrMove, string[] attribs)
        //public void clearSelection()
        //public void setDirectory(string directory)
        //public void printSelectedFiles()
        //public void printAllFilesInDirectory()
        //public string[] getSelectedFiles()
        
        //Function: Get Files
        //@param attribs        array of user specified commands/attributes
        //Gets specific files in the directory which match user specified commands and
        //attributes and stores them in a list
        public void getFiles(string[] attribs) {
            if (directory == null)  {
                Console.WriteLine("Directory has not been set.");
                return;
            }
            if (attribs.Length == 0) {
                error("filemanager getfiles", "attribamt");
                return;
            }
            Console.WriteLine("Acquiring files...");
            //Format: <directory>/<file name>.<file extension>
            string[] filesInDir = Directory.GetFiles(directory); //gets all files
            int numOfFilesSelected = 0, selectedFileCount = selectedFiles.Count;

            for (int i = 0; i < filesInDir.Length; i++) {
                bool selectFile = false;
                string fileName = "";
                try {
                    //Format: <file name>
                    fileName = Path.GetFileNameWithoutExtension(filesInDir[i]);
                } catch (Exception) {
                    error(null, "Unable to find specified directory");
                    return;
                }

                attribs[0] = HelpCommands.getFullCommand(attribs[0]);
                switch(attribs[0]) {
                    //Documentation for commands can be found by executing the "help"
                    //command while in the console
                    case "all":
                        selectFile = true;
                        break;
                    case "name":
                        //Accepts one or more names to be searched
                        if (attribs.Length >= 2) {
                            for (int j = 1; j < attribs.Length; j++)
                                selectFile = fileName == attribs[j];
                        } else {
                            error("filemanager getfiles " + attribs[0], "attribamt");
                            return;
                        }
                        break;
                    case "equals":
                    case "contains":
                        if (attribs.Length == 2) {
                            selectFile = fileName.Contains(attribs[1]);
                        } else if (attribs.Length == 3 || attribs.Length == 4) {
                            int[] beginAndEnd = getBeginEnd(attribs, fileName);
                            if (beginAndEnd == null) return;
                            //Indices specifying the bounds of sub string that is
                            //being worked on
                            int begin = beginAndEnd[0], end = beginAndEnd[1];

                            if (attribs[0] == "equals")
                                selectFile = fileName.Substring(begin, end) == attribs[attribs.Length - 1];
                            if (attribs[0] == "contains")
                                selectFile = fileName.Substring(begin, end).Contains(attribs[attribs.Length - 1]);
                        } else {
                            error("filemanager getfiles " + attribs[0], "attribamt");
                            return;
                        }
                        break;
                    case "date":
                        if (attribs.Length >= 4) {
                            attribs[1] = HelpCommands.getFullCommand(attribs[1]);
                            attribs[2] = HelpCommands.getFullCommand(attribs[2]);
                            int fileDate = 0;
                            if (attribs[1] == "created") {
                                fileDate = Int32.Parse(File.GetCreationTime(filesInDir[i]).ToString("yyyymmdd"));
                            } else if (attribs[1] == "modified") {
                                fileDate = Int32.Parse(File.GetLastWriteTime(filesInDir[i]).ToString("yyyymmdd"));
                            } else {
                                error("filemanager getfiles " + attribs[0], "attrib");
                                return;
                            }

                            //User input: yyyy-mm-dd  ->  yyyymmdd  ->  number form
                            int[] inputDates = new int[attribs.Length - 3];
                            try {
                                for (int j = 3; j < attribs.Length; j++) {
                                    inputDates[j - 3] = Int32.Parse(removeOccurrences(attribs[j], "-"));
                                }
                            } catch (FormatException) {
                                error(null, "invalid date format detected, please use [YYYY-MM-DD]");
                                return;
                            }

                            //Example comparison: the number 20190201 (which is 2019
                            //Feb 1st) is bigger than 20190101 (2019 Jan 1st)
                            if (attribs[2] == "equals") {
                                for (int j = 0; j < inputDates.Length; j++)
                                    selectFile = inputDates[i] == fileDate;
                            } else if (attribs[2] == "after") {
                                selectFile = inputDates[0] < fileDate;
                            } else if (attribs[2] == "before") {
                                selectFile = inputDates[0] > fileDate;
                            } else {
                                error("filemanager getfiles " + attribs[0] + " " + attribs[1], "attrib");
                                return;
                            }
                        } else {
                            error("filemanager getfiles " + attribs[0], "attribamt");
                            return;
                        }
                        break;
                    case "extension":
                        //Extension means file type, eg. <.pdf>, <.txt>, etc
                        //Accepts one or more extensions to be searched
                        if (attribs.Length >= 2) {
                            for (int j = 1; j < attribs.Length; j++)
                                selectFile = Path.GetExtension(filesInDir[i]) == ("." + attribs[j]);
                        } else {
                            error("filemanager getfiles " + attribs[0], "attribamt");
                            return;
                        }
                        break;
                    default:
                        error("filemanager getfiles", "attrib");
                        return;
                }
                if (selectFile) {
                    bool alreadySelected = false;
                    foreach (string file in selectedFiles)
                        if (file == filesInDir[i])
                            alreadySelected = true;
                    if (!alreadySelected) {
                        selectedFiles.Add(filesInDir[i]);
                        Console.WriteLine("<" + Path.GetFileName(selectedFiles[selectedFileCount + i]) + "> acquired.");
                        numOfFilesSelected++;
                    }
                }
            }
            if (numOfFilesSelected != 0)
                Console.WriteLine("(" + numOfFilesSelected + ") file(s) acquired successfully.");
            else Console.WriteLine("No files match the specifications or have already been selected.");
        }

        //Function: Edit Names
        //@param attribs        array of user specified commands/attributes
        //Changes the names of selected files according to user specified commands and
        //attributes
        public void editNames(string [] attribs) {
            if (selectedFiles.Count == 0) {
                Console.WriteLine("No files have been selected.");
                return;
            }
            Console.WriteLine("Editing file names...");
            string[] newFileNames = new string[selectedFiles.Count];
            int[] repeatFileCount = new int[selectedFiles.Count];
            int numOfFilesChanged = 0;

            if (attribs.Length >= 1) {
                for (int i = 0; i < selectedFiles.Count; i++) {
                    string fileName = Path.GetFileNameWithoutExtension(selectedFiles[i]);
                    attribs[0] = HelpCommands.getFullCommand(attribs[0]);
                    switch(attribs[0]) {
                        //Documentation for commands can be found by executing the 
                        //"help" command while in the console
                        case "insert":
                            if (attribs.Length == 3) {
                                int insertIndex = 0;
                                try {
                                    insertIndex = Int32.Parse(attribs[1]);
                                } catch (FormatException) {
                                    error("filemanager editnames " + attribs[0], "attribnum");
                                    return;
                                }
                                if (insertIndex < 0) insertIndex = 0;
                                if (insertIndex > fileName.Length) 
                                    insertIndex = fileName.Length;

                                newFileNames[i] = fileName.Substring(0, insertIndex) + attribs[2] + fileName.Substring(insertIndex);
                            } else {
                                error("filemanager editnames " + attribs[0], "attribamt");
                                return;
                            }
                            break;
                        case "replace":
                            if (attribs.Length >= 3) {
                                int[] beginAndEnd = getBeginEnd(attribs, fileName);
                                if (beginAndEnd == null) return;
                                int begin = beginAndEnd[0], end = beginAndEnd[1];
                                Console.WriteLine(begin + " " + end + " " + fileName);

                                newFileNames[i] = fileName.Substring(0, begin) + (attribs.Length == 3 ? attribs[2] : attribs[3]) + fileName.Substring(end);
                            } else {
                                error("filemanager editnames " + attribs[0], "attribamt");
                                return;
                            }
                            break;
                        case "replaceoccurrences":
                            if (attribs.Length == 3) {
                                newFileNames[i] = replaceOccurrences(fileName, attribs[1], attribs[2]);
                            } else {
                                error("filemanager editnames " + attribs[0], "attribamt");
                                return;
                            }
                            break;
                        case "removeoccurences":
                            if (attribs.Length == 2) {
                                newFileNames[i] = removeOccurrences(fileName, attribs[1]);
                            } else {
                                error("filemanager editnames " + attribs[0], "attribamt");
                                return;
                            }
                            break;
                        case "set":
                            if (attribs.Length == 2) {
                                newFileNames[i] = attribs[1];
                            } else {
                                error("filemanager editnames " + attribs[0], "attribamt");
                                return;
                            }
                            break;
                        default:
                            error("filemanager editnames", "attrib");
                            return;
                    }

                    if (newFileNames[i] == null || newFileNames[i].Length == 0) {
                        error(null, "something went wrong while changing file name, cancelling operation.");
                        return;
                    }

                    for (int j = 0; j < i; j++) {
                        if (newFileNames[i] == newFileNames[j] && Path.GetDirectoryName(selectedFiles[i]) == Path.GetDirectoryName(selectedFiles[j]))
                            newFileNames[i] = newFileNames[i] + "(" + (++repeatFileCount[j]) + ")";
                    }

                    string newFile = Path.GetDirectoryName(selectedFiles[i]) + "\\" + newFileNames[i] + Path.GetExtension(selectedFiles[i]);
                    if (!File.Exists(newFile)) {
                        System.IO.File.Move(selectedFiles[i], newFile);
                        if (selectedFiles[i] != newFile) {
                            Console.WriteLine("<" + Path.GetFileName(selectedFiles[i]) + "> changed to <" + Path.GetFileName(newFile) + ">.");
                            numOfFilesChanged++;
                        }
                        selectedFiles[i] = newFile;
                    } else {
                        Console.WriteLine("<" + Path.GetFileName(selectedFiles[i]) + "> could not be changed to <" + Path.GetFileName(newFile) + "> because there is already a file with that name.");
                    }
                }
            } else {
                error("filemanager editnames", "attribamt");
                return;
            }
            if (numOfFilesChanged != 0)
                Console.WriteLine("(" + numOfFilesChanged + ") file name(s) edited successfully.");
            else Console.WriteLine("No file names match the specifications.");
        }

        //Function Name: Copy Or Move Files To
        //@param copyOrMove         specifies whether the selected files are being
        //                          copied or moved
        //       attribs            user specified commands which provides the 
        //                          destination directory or notand whether to replace
        //                          duplicate files
        //Copies or moves all selected files to a new user specified directory
        public void copyOrMoveFilesTo(string copyOrMove, string[] attribs) {

            if (selectedFiles.Count == 0) {
                Console.WriteLine("No files have been selected.");
                return;
            }
            string[] keywords = copyOrMove == "copyto" ? new string[]{"Copy", "copy", "copied"} : new string[]{"Move", "move", "moved"};
            if (attribs.Length == 1 || attribs.Length == 2) {
                if (directoryExists(attribs[0])) {

                    bool replace = false;
                    int filesCopied = 0, filesReplaced = 0, filesSkipped = 0;
                    string[] destFiles = Directory.GetFiles(attribs[0]);

                    if (attribs.Length == 2) {
                        attribs[1] = HelpCommands.getFullCommand(attribs[1]);
                        if (attribs[1] == "-replace") {
                            Console.Write("Replacing a duplicate file cannot be undone, type \"OK\" to confirm operation: ");
                            string confirmation = Console.ReadLine().ToUpper();
                            if (confirmation == "OK") {
                                Console.WriteLine(keywords[0] + " and replace duplicate files confirmed.");
                                replace = true;
                            } else Console.WriteLine("Keyword not entered, returning to " + keywords[1] + " and skip duplicate files mode.");
                        } else Console.WriteLine("Unknown attribute, continuing with " + keywords[1] + " and skip duplicate files mode.");
                    }

                    for (int i = 0; i < selectedFiles.Count; i++) {
                        try {
                            if (copyOrMove == "copyto") {
                                File.Copy(selectedFiles[i], attribs[0] + "\\" + Path.GetFileName(selectedFiles[i]));
                            } else {
                                File.Move(selectedFiles[i], attribs[0] + "\\" + Path.GetFileName(selectedFiles[i]));
                                selectedFiles[i] = attribs[0] + "\\" + Path.GetFileName(selectedFiles[i]);
                            }
                            Console.WriteLine("<" + Path.GetFileName(selectedFiles[i]) + "> has been "+ keywords[2] + ".");
                            filesCopied++;
                        } catch (System.IO.IOException) {
                            if (replace) {
                                Console.WriteLine("<" + Path.GetFileName(selectedFiles[i]) + "> has been " + keywords[2] + " and has replaced a duplicate file.");
                                filesReplaced++;
                                if (copyOrMove == "copyto") {
                                    File.Copy(selectedFiles[i], attribs[0] + "\\" + Path.GetFileName(selectedFiles[i]), true);
                                } else {
                                    moveFile(selectedFiles[i], attribs[0] + "\\" + Path.GetFileName(selectedFiles[i]), true);
                                    selectedFiles[i] = attribs[0] + "\\" + Path.GetFileName(selectedFiles[i]);
                                }
                            } else {
                                Console.WriteLine("<" + Path.GetFileName(selectedFiles[i]) + "> was not " + keywords[2] + " due to a duplicate file.");
                                filesSkipped++;
                            }
                        }
                    }
                    Console.WriteLine(keywords[0] + " complete: (" + filesCopied + ") files were " + keywords[2] + ", (" + filesReplaced + ") files were replaced and (" + filesSkipped + ") files were skipped");
                } else error(null, "the given directory does not exist");
            } else error("filemanager" + copyOrMove, "attrib");
        }

        //Function Name: Delete
        //Permanently deletes all selected files
        public void delete() {
            if (selectedFiles.Count == 0) {
                Console.WriteLine("No files have been selected.");
                return;
            }
            
            Console.Write("A permanently deleted file cannot be recovered, type \"OK\" to confirm operation: ");
            string confirmation = Console.ReadLine().ToUpper();
            if (confirmation == "OK") {
                Console.WriteLine("Permanent file deletion confirmed.");
                
                foreach (string selectedFile in selectedFiles) {
                    File.Delete(selectedFile);
                    Console.WriteLine("<" + Path.GetFileName(selectedFile) + "> deleted.");
                }
                Console.WriteLine("(" + selectedFiles.Count + ") files were deleted");
                selectedFiles = new List<string>();
            } else Console.WriteLine("Keyword not entered, files will not be permanent deleted.");
        }

        //Function: Clear Selection
        //Removes all files currently in the selected list
        public void clearSelection() {
            selectedFiles = new List<string>();
            Console.WriteLine("File selection cleared.");
        }

        //Function: Set Directory
        //Specifies the directory file path which the File Manager will be working
        //in / getting files from
        public void setDirectory(string directory) {
            if (directory[directory.Length - 1] != '\\' || directory[directory.Length - 1] != '/')
                directory = directory + "\\";
            if (directoryExists(directory)) {
                this.directory = directory;
                Console.WriteLine("Directory set.");
            } else error(null, "the given directory does not exist");
        }

        //Function: Get Directory
        //@return           the file path of the currently selected directory
        public string getDirectory() {
            return directory;
        }

        //Function: Print Selected Files
        //Outputs the list of all selected files
        public void printSelectedFiles() {

            if (selectedFiles.Count == 0) {
                Console.WriteLine("No files have been selected.");
            } else {
                List<string> filesByDirectories = new List<string>();
                List<string> directories = new List<string>();
                
                //Sorts files by directories
                foreach (string selectedFile in selectedFiles) {
                    bool matched = false;
                    int count = 0;
                    while (!matched && count < directories.Count) {
                        if (directories[count] == Path.GetDirectoryName(selectedFile)) {
                            filesByDirectories[count] += "\n<" + Path.GetFileName(selectedFile) + ">";
                            matched = true;
                        }
                        count++;
                    }
                    if (!matched) {
                        directories.Add(Path.GetDirectoryName(selectedFile));
                        filesByDirectories.Add("<" + Path.GetFileName(selectedFile) + ">");
                    }
                }

                Console.WriteLine("Selected files: ");
                for (int i = 0; i < directories.Count; i++) {
                    Console.WriteLine("Under directory: " + directories[i] + "\n" + filesByDirectories[i]);
                }
                Console.WriteLine("(" + selectedFiles.Count + ") file(s) currently selected");
            }
        }

        //Function: Print All Files In Directory
        //Outputs the list of all files in directory
        public void printAllFilesInDirectory() {
            if (directory == null) {
                Console.WriteLine("Directory has not been set.");
                return;
            }
            Console.WriteLine("Directory: " + directory + "\nAll files: ");
            string[] allFilesInDir = Directory.GetFiles(directory);
            if (allFilesInDir.Length == 0) Console.WriteLine("The directory is empty.");
            foreach (string file in allFilesInDir) {
                Console.WriteLine(Path.GetFileName(file));
            }
        }

        //Function: Get Selected Files
        //@return           the selected files as a string array
        public string[] getSelectedFiles() {
            string[] files = new string[selectedFiles.Count];
            for (int i = 0; i < files.Length; i++) {
                files[i] = selectedFiles[i];
            }
            return files;
        }

        //-----------------------------UTILITY FUNCTIONS-----------------------------//
        //FUNCTION LIST:
        //private int[] getBeginEnd(String [] attribs, string fileName)
        //private void error(string command, string reason)

        //Function: Get Begin End
        //@param attribs        attributes containg user specified beginning and ending
        //                      indicies in string form
        //       fileName       name of the current file being processed
        //@return               an integer array of length 2 containing the beginning
        //                      and ending indices
        //Checks the user inputted indices for any mistakes and attempts to catch
        //and correct (if possible) them before returning them in integer form
        private int[] getBeginEnd(String [] attribs, string fileName) {
            Console.WriteLine("DEBUG");
            int begin = 0, end = 0;
            try {
                begin = Int32.Parse(attribs[1]);
                if (attribs.Length == 3) {
                    //A single negative index given means counting from the end of
                    //file name
                    //Eg. filename: <document_file> index: -4
                    //             interval:  ^^^^
                    end = fileName.Length;
                    if (begin < 0)
                        begin = fileName.Length - 1 - begin;
                }
                if (attribs.Length == 4) {
                    end = Int32.Parse(attribs[2]);
                }
            } catch (FormatException) {
                error(null, "attribnum");
                return null;
            }
            //Correcting the indices if they are invalid
            if (begin < 0) begin = 0;
            if (end > fileName.Length) end = fileName.Length;
            if (begin > end) {
                int temp = begin;
                begin = end;
                end = temp;
            }
            return new int[]{begin, end};
        }

        //Function: Error
        //@param command        The command which produced the error
        //       reason         The cause for the error
        //Prints out an error message for when the user inputs an incorrect
        //command and displays the correct way to execute the command
        private void error(string command, string reason) {
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

        //Function Name: Directory Exists
        //@param directory      The path of the file directory
        //@return               Whether the directory exists or not
        private bool directoryExists(String directory) {
            try {
                Directory.GetFiles(directory);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        //Function Name: Move File
        //@param filePath       The original path of file
        //       destPath       Path of destination directory and file name
        //       replace        Specifies whether duplicate files will be
        //                      replaced or not
        //Moves file to new directory, if replace is set to true, it will
        //delete duplicate files in the destination directory and move
        //the new file there
        private void moveFile(String filePath, String destPath, bool replace) {
            if (File.Exists(destPath) && replace) File.Delete(destPath);
            File.Move(filePath, destPath);
        }

        //-----------------------------ASSIST FUNCTIONS-----------------------------//
        //FUNCTION LIST:
        //private string removeOccurrences(string str, string sub)
        //private string replaceOccurrences(string str, string find, string replace)

        //Function: Remove Occurrences
        //@param str        the string for which to perform the search
        //       sub        the sub string which is to be removed from <str>
        //@return           a new string with all instances of <sub> removed from 
        //                  <str>
        private string removeOccurrences(string str, string sub) {
            return replaceOccurrences(str, sub, "");
        }

        //Function: Replace Occurrences
        //@param str        the string for which to perform the search
        //       find       the string to be replaced
        //       replace    the string to replace it with
        //@return           a new string with all instances of <find> replaced with
        //                  <replace>
        private string replaceOccurrences(string str, string find, string replace) {
            if (str.Length < find.Length) return str;
            if (str.Substring(0, find.Length).Equals(find))
                return replace + replaceOccurrences(str.Substring(find.Length), find, replace);
            return str[0] + replaceOccurrences(str.Substring(1), find ,replace);
        }
    }

}