
.assembly extern System.Runtime
{
  .publickeytoken = ( B0 3F 5F 7F 11 D5 0A 3A ) // .._.....
  .ver 10:0:0:0
}

.assembly extern System.Console
{
  .publickeytoken = ( B0 3F 5F 7F 11 D5 0A 3A ) // .._.....
  .ver 10:0:0:0
}

.assembly 'Selsort'
{
  .custom instance void class [System.Runtime]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) // ........
  .custom instance void class [System.Runtime]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78 63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 ) // ....T..WrapNonExceptionThrows.
  .custom instance void class [System.Runtime]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 19 2E 4E 45 54 43 6F 72 65 41 70 70 2C 56 65 72 73 69 6F 6E 3D 76 31 30 2E 30 01 00 54 0E 14 46 72 61 6D 65 77 6F 72 6B 44 69 73 70 6C 61 79 4E 61 6D 65 09 2E 4E 45 54 20 31 30 2E 30 ) // ....NETCoreApp.Version.v10.0..T..FrameworkDisplayName..NET.10.0
  .custom instance void class [System.Runtime]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 07 53 65 6C 73 6F 72 74 00 00 ) // ...Selsort..
  .custom instance void class [System.Runtime]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 07 52 65 6C 65 61 73 65 00 00 ) // ...Release..
  .custom instance void class [System.Runtime]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 ) // ...1.0.0.0..
  .custom instance void class [System.Runtime]System.Reflection.AssemblyInformationalVersionAttribute::.ctor(string) = ( 01 00 2E 31 2E 30 2E 30 2B 38 35 30 34 37 35 33 37 65 61 38 34 66 30 39 65 66 39 61 36 32 65 32 64 39 33 30 64 62 61 61 35 66 30 38 63 32 65 61 61 00 00 ) // ...1.0.0.85047537ea84f09ef9a62e2d930dbaa5f08c2eaa..
  .custom instance void class [System.Runtime]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 07 53 65 6C 73 6F 72 74 00 00 ) // ...Selsort..
  .custom instance void class [System.Runtime]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 07 53 65 6C 73 6F 72 74 00 00 ) // ...Selsort..
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}

.module 'Selsort.dll'
// MVID: {43ca4549-c877-4bd4-9a51-85817cc72534}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003  // WindowsCui
.corflags 0x00000001  // ILOnly

.class private auto ansi beforefieldinit Selsort extends [System.Runtime]System.Object
{
  .custom instance void class [System.Runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(byte) = ( 01 00 01 00 00 ) // .....
  .custom instance void class [System.Runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(byte) = ( 01 00 00 00 00 ) // .....
  .field public static initonly class [System.Runtime]System.Random rnd
  .method public hidebysig static default void Main(string[] args) cil managed
  {
    // Method begins at Relative Virtual Address (RVA) 0x2050
    .entrypoint
    // Code size 91 (0x5B)
    .maxstack 4
    .locals init(int32 V_0, int32[] V_1, int32 V_2, int32 V_3)
    IL_0000: ldc.i4.s 10
    IL_0002: stloc.0
    IL_0003: ldloc.0
    IL_0004: newarr class System.Int32
    IL_0009: stloc.1
    IL_000a: ldc.i4.0
    IL_000b: stloc.2
    IL_000c: br.s     IL_0024
    IL_000e: ldloc.1
    IL_000f: ldloc.2
    IL_0010: ldsfld class [System.Runtime]System.Random Selsort::rnd
    IL_0015: ldc.i4 1000000
    IL_001a: callvirt instance int32 class [System.Runtime]System.Random::Next(int32)
    IL_001f: stelem.i4
    IL_0020: ldloc.2
    IL_0021: ldc.i4.1
    IL_0022: add
    IL_0023: stloc.2
    IL_0024: ldloc.2
    IL_0025: ldloc.0
    IL_0026: blt.s     IL_000e
    IL_0028: ldloc.1
    IL_0029: call void class Selsort::SelectionSort(int32[])
    IL_002e: ldc.i4.0
    IL_002f: stloc.3
    IL_0030: br.s     IL_0051
    IL_0032: ldloc.1
    IL_0033: ldloc.3
    IL_0034: ldelema class System.Int32
    IL_0039: call instance string class int32::ToString()
    IL_003e: ldstr " "
    IL_0043: call string class string::Concat(string, string)
    IL_0048: call void class [System.Console]System.Console::Write(string)
    IL_004d: ldloc.3
    IL_004e: ldc.i4.1
    IL_004f: add
    IL_0050: stloc.3
    IL_0051: ldloc.3
    IL_0052: ldloc.0
    IL_0053: blt.s     IL_0032
    IL_0055: call void class [System.Console]System.Console::WriteLine()
    IL_005a: ret
  } // End of method System.Void Selsort::Main(System.String[])
  .method public hidebysig static default void SelectionSort(int32[] arr) cil managed
  {
    // Method begins at Relative Virtual Address (RVA) 0x20B8
    // Code size 57 (0x39)
    .maxstack 4
    .locals init(int32 V_0, int32 V_1, int32 V_2, int32 V_3)
    IL_0000: ldc.i4.0
    IL_0001: stloc.0
    IL_0002: br.s     IL_0032
    IL_0004: ldloc.0
    IL_0005: stloc.1
    IL_0006: ldloc.0
    IL_0007: ldc.i4.1
    IL_0008: add
    IL_0009: stloc.3
    IL_000a: br.s     IL_001a
    IL_000c: ldarg.0
    IL_000d: ldloc.3
    IL_000e: ldelem.i4
    IL_000f: ldarg.0
    IL_0010: ldloc.1
    IL_0011: ldelem.i4
    IL_0012: bge.s     IL_0016
    IL_0014: ldloc.3
    IL_0015: stloc.1
    IL_0016: ldloc.3
    IL_0017: ldc.i4.1
    IL_0018: add
    IL_0019: stloc.3
    IL_001a: ldloc.3
    IL_001b: ldarg.0
    IL_001c: ldlen
    IL_001d: conv.i4
    IL_001e: blt.s     IL_000c
    IL_0020: ldarg.0
    IL_0021: ldloc.0
    IL_0022: ldelem.i4
    IL_0023: stloc.2
    IL_0024: ldarg.0
    IL_0025: ldloc.0
    IL_0026: ldarg.0
    IL_0027: ldloc.1
    IL_0028: ldelem.i4
    IL_0029: stelem.i4
    IL_002a: ldarg.0
    IL_002b: ldloc.1
    IL_002c: ldloc.2
    IL_002d: stelem.i4
    IL_002e: ldloc.0
    IL_002f: ldc.i4.1
    IL_0030: add
    IL_0031: stloc.0
    IL_0032: ldloc.0
    IL_0033: ldarg.0
    IL_0034: ldlen
    IL_0035: conv.i4
    IL_0036: blt.s     IL_0004
    IL_0038: ret
  } // End of method System.Void Selsort::SelectionSort(System.Int32[])
  .method public hidebysig specialname rtspecialname instance default void .ctor() cil managed
  {
    // Method begins at Relative Virtual Address (RVA) 0x20FD
    // Code size 7 (0x7)
    .maxstack 8
    IL_0000: ldarg.0
    IL_0001: call instance void class [System.Runtime]System.Object::.ctor()
    IL_0006: ret
  } // End of method System.Void Selsort::.ctor()
  .method private hidebysig specialname rtspecialname static default void .cctor() cil managed
  {
    // Method begins at Relative Virtual Address (RVA) 0x2105
    // Code size 11 (0xB)
    .maxstack 8
    IL_0000: newobj instance void class [System.Runtime]System.Random::.ctor()
    IL_0005: stsfld class [System.Runtime]System.Random Selsort::rnd
    IL_000a: ret
  } // End of method System.Void Selsort::.cctor()
} // End of class Selsort
