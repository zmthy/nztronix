using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using tape.data;

namespace tape.io {

  class FileReader {

     public BinaryData ReadBinFile(String Location) {
       StreamReader reader = new StreamReader(Location);
       List<bool> fileData = new List<bool>();

       try {
         do {
           foreach (Char c in reader.ReadLine()) {
             if (c.Equals("1")) {
               fileData.Add(true);
             } else if (c.Equals("0")) {
               fileData.Add(false);
             } else {
               throw new Exception("Invalid file contents.");
             }
           }
         } while (reader.Peek() != -1);
       } catch {}
       
       reader.Close();
       return new BinaryData(fileData);
    }

  }

}
