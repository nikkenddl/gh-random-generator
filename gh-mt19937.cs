using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino.Collections;

using GH_IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;



/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public class Script_Instance : GH_ScriptInstance
{
#region Utility functions
  /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
  /// <param name="text">String to print.</param>
  private void Print(string text) { /* Implementation hidden. */ }
  /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
  /// <param name="format">String format.</param>
  /// <param name="args">Formatting parameters.</param>
  private void Print(string format, params object[] args) { /* Implementation hidden. */ }
  /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj) { /* Implementation hidden. */ }
  /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj, string method_name) { /* Implementation hidden. */ }
#endregion

#region Members
  /// <summary>Gets the current Rhino document.</summary>
  private readonly RhinoDoc RhinoDocument;
  /// <summary>Gets the Grasshopper document that owns this script.</summary>
  private readonly GH_Document GrasshopperDocument;
  /// <summary>Gets the Grasshopper script component that owns this script.</summary>
  private readonly IGH_Component Component;
  /// <summary>
  /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
  /// Any subsequent call within the same solution will increment the Iteration count.
  /// </summary>
  private readonly int Iteration;
#endregion

  /// <summary>
  /// This procedure contains the user code. Input parameters are provided as regular arguments,
  /// Output parameters as ref arguments. You don't have to assign output parameters,
  /// they will have a default value.
  /// </summary>
  private void RunScript(int nRand, int seed, ref object result)
  {

    init_genrand((uint) seed);

    var _result = new List<double>();
    for (int i = 0; i < nRand; i++) {
      _result.Add(genrand_res53());
    }

    result = _result;

  }

  // <Custom additional code> 


  /* ---- MODIFIED ORIGINAL CODE ---- */

  /*
  A C-program for MT19937, with initialization improved 2002/1/26.
  Coded by Takuji Nishimura and Makoto Matsumoto.

  Before using, initialize the state by using init_genrand(seed)
  or init_by_array(init_key, key_length).

  Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
  All rights reserved.

  Redistribution and use in source and binary forms, with or without
  modification, are permitted provided that the following conditions
  are met:

  1. Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

  2. Redistributions in binary form must reproduce the above copyright
  notice, this list of conditions and the following disclaimer in the
  documentation and/or other materials provided with the distribution.

  3. The names of its contributors may not be used to endorse or promote
  products derived from this software without specific prior written
  permission.

  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
  A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


  Any feedback is very welcome.
  http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
  email: m-mat @ math.sci.hiroshima-u.ac.jp (remove space)
  */

  /* Period parameters */
  static uint N = 624;
  static uint M = 397;
  static ulong MATRIX_A = 0x9908b0dfUL;   /* constant vector a */
  static ulong UPPER_MASK = 0x80000000UL; /* most significant w-r bits */
  static ulong LOWER_MASK = 0x7fffffffUL; /* least significant r bits */

  static ulong[] mt = new ulong[N]; /* the array for the state vector  */
  static uint mti = N + 1; /* mti==N+1 means mt[N] is not initialized */

  /* initializes mt[N] with a seed */
  void init_genrand(ulong s)
  {
    mt[0] = s & 0xffffffffUL;
    for (mti = 1; mti < N; mti++) {
      mt[mti] =
        (1812433253UL * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti);
      /* See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier. */
      /* In the previous versions, MSBs of the seed affect   */
      /* only MSBs of the array mt[].                        */
      /* 2002/01/09 modified by Makoto Matsumoto             */
      mt[mti] &= 0xffffffffUL;
      /* for >32 bit machines */
    }
  }

  /* initialize by an array with array-length */
  /* init_key is the array for initializing keys */
  /* key_length is its length */
  /* slight change for C++, 2004/2/26 */
  void init_by_array(ulong[] init_key, uint key_length)
  {
    uint i, j, k;
    init_genrand(19650218UL);
    i = 1; j = 0;
    k = (N > key_length ? N : key_length);
    for (; k > 0; k--) {
      mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1664525UL))
        + init_key[j] + j; /* non linear */
      mt[i] &= 0xffffffffUL; /* for WORDSIZE > 32 machines */
      i++; j++;
      if (i >= N) { mt[0] = mt[N - 1]; i = 1; }
      if (j >= key_length) j = 0;
    }
    for (k = N - 1; k > 0; k--) {
      mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1566083941UL))
        - i; /* non linear */
      mt[i] &= 0xffffffffUL; /* for WORDSIZE > 32 machines */
      i++;
      if (i >= N) { mt[0] = mt[N - 1]; i = 1; }
    }

    mt[0] = 0x80000000UL; /* MSB is 1; assuring non-zero initial array */
  }

  /* generates a random number on [0,0xffffffff]-interval */
  ulong genrand_int32()
  {
    ulong y;
    ulong[] mag01 = {0x0UL, MATRIX_A};
  /* mag01[x] = x * MATRIX_A  for x=0,1 */

  if (mti >= N) { /* generate N words at one time */
  int kk;

  if (mti == N+1)   /* if init_genrand() has not been called, */
  init_genrand(5489UL); /* a default initial seed is used */

  for (kk=0;kk<N-M;kk++) {
  y = (mt[kk]&UPPER_MASK)|(mt[kk+1]&LOWER_MASK);
  mt[kk] = mt[kk+M] ^ (y >> 1) ^ mag01[y & 0x1UL];
}
        for (;kk<N-1;kk++) {
            y = (mt[kk]&UPPER_MASK)|(mt[kk+1]&LOWER_MASK);
            mt[kk] = mt[(int)(kk+(M-N))] ^ (y >> 1) ^ mag01[y & 0x1UL];
        }
        y = (mt[N-1]&UPPER_MASK)|(mt[0]&LOWER_MASK);
        mt[N-1] = mt[M-1] ^ (y >> 1) ^ mag01[y & 0x1UL];

        mti = 0;
    }

    y = mt[mti++];

    /* Tempering */
    y ^= (y >> 11);
    y ^= (y << 7) & 0x9d2c5680UL;
    y ^= (y << 15) & 0xefc60000UL;
    y ^= (y >> 18);

    return y;
}

/* generates a random number on [0,0x7fffffff]-interval */
long genrand_int31()
{
  return (long) (genrand_int32() >> 1);
}

/* generates a random number on [0,1]-real-interval */
double genrand_real1()
{
  return genrand_int32() * (1.0 / 4294967295.0);
  /* divided by 2^32-1 */
}

/* generates a random number on [0,1)-real-interval */
double genrand_real2()
{
  return genrand_int32() * (1.0 / 4294967296.0);
  /* divided by 2^32 */
}

/* generates a random number on (0,1)-real-interval */
double genrand_real3()
{
  return (((double) genrand_int32()) + 0.5) * (1.0 / 4294967296.0);
  /* divided by 2^32 */
}

/* generates a random number on [0,1) with 53-bit resolution*/
double genrand_res53()
{
  ulong a=genrand_int32() >> 5, b = genrand_int32() >> 6;
  return(a * 67108864.0 + b) * (1.0 / 9007199254740992.0);
}
/* These real versions are due to Isaku Wada, 2002/01/09 added */

int main()
{
  int i;
  ulong[] init = {0x123UL, 0x234UL, 0x345UL, 0x456UL};
uint length = 4U;
  init_by_array(init, length);
  Print("1000 outputs of genrand_int32()\n");
  for (i = 0; i < 1000; i++) {
    Print("{0}", genrand_int32());
    if (i % 5 == 4) Print("\n");
  }
  Print("\n1000 outputs of genrand_real2()\n");
  for (i = 0; i < 1000; i++) {
    Print("{0}", genrand_real2());
    if (i % 5 == 4) Print("\n");
  }
  return 0;
}

  // </Custom additional code> 
}
