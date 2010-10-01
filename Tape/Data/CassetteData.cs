﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Tape.Data {

  /// <summary>
  /// Represents the data on the cassette in object form.
  /// </summary>
  /// 
  /// <author>Timothy Jones</author>
  public class CassetteData : IEnumerable<byte> {

    public readonly String Filename;
    private readonly ByteData Program;
    public int Length {
      get {
        return Program.Length;
      }
    }

    public CassetteData(string filename, ByteData program) {
      Filename = filename;
      Program = program;
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return (IEnumerator) GetEnumerator();
    }

    public IEnumerator<byte> GetEnumerator() {
      return Program.GetEnumerator();
    }

  }

}