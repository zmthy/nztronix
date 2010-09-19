using System;
using System.Collections.Generic;
using tape.data;

namespace tape.pipeline {
  
    //TODO talk to Tim about informatioon required to finish method
  public class WavGenerator {
      private Int16[] one = {1000, 0, 1000, 0 };
      private Int16[] zero = {600, 600, 0 , 0 };
      //Shound be 2-4 seconds our default sample rate is 400000 ish samples per secon
      private int silenceBufLen = 1600000;

    //Load from a the binary data. The main one used in the pipeline
    public SoundData CreateWav(BinaryData data) {
     // List<Int16> sData = new List<Int16>();
        Int16[] sData = new Int16[((data.Length*4)+ silenceBufLen)];
        for(int i = 0; i < silenceBufLen; i++){
            sData[i] = 0;        
        }

        int length = data.Length + silenceBufLen;
        int index = silenceBufLen;
        foreach(bool j in data)
        {
            if(j){
                foreach(Int16 num in one){
                    sData[index] = num;
                    
                }
            }
            else{
                 foreach(Int16 num in zero){
                    sData[index] = num;
                    
                }
            }
        }
       
        //return new SoundData() -- 
        return null;
    }

    //Load from a .bin file
    //For recovering purposes
    public SoundData CreateWav(String location)
    {
        return null;
    }

    
    
  }
  
}
