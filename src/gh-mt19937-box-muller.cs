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
  private void Print(string text) { __out.Add(text); }
  /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
  /// <param name="format">String format.</param>
  /// <param name="args">Formatting parameters.</param>
  private void Print(string format, params object[] args) { __out.Add(string.Format(format, args)); }
  /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj) { __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj)); }
  /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj, string method_name) { __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj, method_name)); }
#endregion

#region Members
  /// <summary>Gets the current Rhino document.</summary>
  private RhinoDoc RhinoDocument;
  /// <summary>Gets the Grasshopper document that owns this script.</summary>
  private GH_Document GrasshopperDocument;
  /// <summary>Gets the Grasshopper script component that owns this script.</summary>
  private IGH_Component Component; 
  /// <summary>
  /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
  /// Any subsequent call within the same solution will increment the Iteration count.
  /// </summary>
  private int Iteration;
#endregion

  /// <summary>
  /// This procedure contains the user code. Input parameters are provided as regular arguments, 
  /// Output parameters as ref arguments. You don't have to assign output parameters, 
  /// they will have a default value.
  /// </summary>
  private void RunScript(int n, System.Object sigma, double mu, int seed, ref object normal1, ref object normal2, ref object uniform1, ref object uniform2)
  {
    
    double _sigma = 1;
    if (sigma != null) {
      _sigma = Convert.ToDouble(sigma);
    }


    // Generate 2 uniform distributions.
    init_genrand((uint) seed);
    var u1 = genUniformRandom(n);
    var u2 = genUniformRandom(n);

    // Box-Muller method.
    // Generates 2 normal distributions from 2 uniform distributions.
    double[] result1 = new double[n];
    double[] result2 = new double[n];
    for (int i = 0; i < n; i++) {
      result1[i] = mu + _sigma * Math.Sqrt(-2.0 * Math.Log(u1[i])) * Math.Sin(2.0 * Math.PI * u2[i]);
      result2[i] = mu + _sigma * Math.Sqrt(-2.0 * Math.Log(u1[i])) * Math.Cos(2.0 * Math.PI * u2[i]);
    }

    // Result.
    uniform1 = u1;
    uniform2 = u2;
    normal1 = result1;
    normal2 = result2;
  }

  // <Custom additional code> 
  
  private double[] genUniformRandom(int n) {
    double[] result = new double[n];
    for (int i = 0; i < n; i++) {
      result[i] = genrand_res53();
    }
    return (result);
  }

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
  const uint N = 624;
  const uint M = 397;
  const ulong MATRIX_A = 0x9908b0dfUL;   /* constant vector a */
  const ulong UPPER_MASK = 0x80000000UL; /* most significant w-r bits */
  const ulong LOWER_MASK = 0x7fffffffUL; /* least significant r bits */

  ulong[] mt = new ulong[N]; /* the array for the state vector  */
  uint mti = N + 1; /* mti==N+1 means mt[N] is not initialized */

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
            mt[kk] = mt[unchecked((int)(kk+(M-N)))] ^ (y >> 1) ^ mag01[y & 0x1UL];
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

  // </Custom additional code> 

  private List<string> __err = new List<string>(); //Do not modify this list directly.
  private List<string> __out = new List<string>(); //Do not modify this list directly.
  private RhinoDoc doc = RhinoDoc.ActiveDoc;       //Legacy field.
  private IGH_ActiveObject owner;                  //Legacy field.
  private int runCount;                            //Legacy field.
  
  public override void InvokeRunScript(IGH_Component owner, object rhinoDocument, int iteration, List<object> inputs, IGH_DataAccess DA)
  {
    //Prepare for a new run...
    //1. Reset lists
    this.__out.Clear();
    this.__err.Clear();

    this.Component = owner;
    this.Iteration = iteration;
    this.GrasshopperDocument = owner.OnPingDocument();
    this.RhinoDocument = rhinoDocument as Rhino.RhinoDoc;

    this.owner = this.Component;
    this.runCount = this.Iteration;
    this. doc = this.RhinoDocument;

    //2. Assign input parameters
        int n = default(int);
    if (inputs[0] != null)
    {
      n = (int)(inputs[0]);
    }

    System.Object sigma = default(System.Object);
    if (inputs[1] != null)
    {
      sigma = (System.Object)(inputs[1]);
    }

    double mu = default(double);
    if (inputs[2] != null)
    {
      mu = (double)(inputs[2]);
    }

    int seed = default(int);
    if (inputs[3] != null)
    {
      seed = (int)(inputs[3]);
    }



    //3. Declare output parameters
      object normal1 = null;
  object normal2 = null;
  object uniform1 = null;
  object uniform2 = null;


    //4. Invoke RunScript
    RunScript(n, sigma, mu, seed, ref normal1, ref normal2, ref uniform1, ref uniform2);
      
    try
    {
      //5. Assign output parameters to component...
            if (normal1 != null)
      {
        if (GH_Format.TreatAsCollection(normal1))
        {
          IEnumerable __enum_normal1 = (IEnumerable)(normal1);
          DA.SetDataList(1, __enum_normal1);
        }
        else
        {
          if (normal1 is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(1, (Grasshopper.Kernel.Data.IGH_DataTree)(normal1));
          }
          else
          {
            //assign direct
            DA.SetData(1, normal1);
          }
        }
      }
      else
      {
        DA.SetData(1, null);
      }
      if (normal2 != null)
      {
        if (GH_Format.TreatAsCollection(normal2))
        {
          IEnumerable __enum_normal2 = (IEnumerable)(normal2);
          DA.SetDataList(2, __enum_normal2);
        }
        else
        {
          if (normal2 is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(2, (Grasshopper.Kernel.Data.IGH_DataTree)(normal2));
          }
          else
          {
            //assign direct
            DA.SetData(2, normal2);
          }
        }
      }
      else
      {
        DA.SetData(2, null);
      }
      if (uniform1 != null)
      {
        if (GH_Format.TreatAsCollection(uniform1))
        {
          IEnumerable __enum_uniform1 = (IEnumerable)(uniform1);
          DA.SetDataList(3, __enum_uniform1);
        }
        else
        {
          if (uniform1 is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(3, (Grasshopper.Kernel.Data.IGH_DataTree)(uniform1));
          }
          else
          {
            //assign direct
            DA.SetData(3, uniform1);
          }
        }
      }
      else
      {
        DA.SetData(3, null);
      }
      if (uniform2 != null)
      {
        if (GH_Format.TreatAsCollection(uniform2))
        {
          IEnumerable __enum_uniform2 = (IEnumerable)(uniform2);
          DA.SetDataList(4, __enum_uniform2);
        }
        else
        {
          if (uniform2 is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(4, (Grasshopper.Kernel.Data.IGH_DataTree)(uniform2));
          }
          else
          {
            //assign direct
            DA.SetData(4, uniform2);
          }
        }
      }
      else
      {
        DA.SetData(4, null);
      }

    }
    catch (Exception ex)
    {
      this.__err.Add(string.Format("Script exception: {0}", ex.Message));
    }
    finally
    {
      //Add errors and messages... 
      if (owner.Params.Output.Count > 0)
      {
        if (owner.Params.Output[0] is Grasshopper.Kernel.Parameters.Param_String)
        {
          List<string> __errors_plus_messages = new List<string>();
          if (this.__err != null) { __errors_plus_messages.AddRange(this.__err); }
          if (this.__out != null) { __errors_plus_messages.AddRange(this.__out); }
          if (__errors_plus_messages.Count > 0) 
            DA.SetDataList(0, __errors_plus_messages);
        }
      }
    }
  }
}