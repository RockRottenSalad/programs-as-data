��	.method public hidebysig static 
		void SelectionSort (
			int32[] arr
		) cil managed 
	{
		// Method begins at RVA 0x20cc
		// Header size: 12
		// Code size: 78 (0x4e)
		.maxstack 4
		.locals init (
			[0] int32,
			[1] int32,
			[2] int32,
			[3] int32,
			[4] bool,
			[5] bool,
			[6] bool
		)

		IL_0000: nopx
		IL_0001: ldc.i4.0  					// push constant to stack
		IL_0002: stloc.0   					// pop stacktop into local variable ( i = 0)
		IL_0003: br.s IL_0041 //enter loop?
		// loop start (head: IL_0041)
			IL_0005: nop
			IL_0006: ldloc.0 				// push int i = 0 (local 0 = i)
			IL_0007: stloc.1 				// push int least = i (local 1 = least)
			IL_0008: ldloc.0 				// push int i
			IL_0009: ldc.i4.1 				// push int of value 1
			IL_000a: add 					// compute i + 1
			IL_000b: stloc.3 				// j = i + 1 (assign result of i + 1 = j)
			IL_000c: br.s IL_0022           
			// loop start (head: IL_0022)
				IL_000e: ldarg.0 			// push arr
				IL_000f: ldloc.3 			// push j (local 3 = j)
				IL_0010: ldelem.i4 			// push arr[j]
				IL_0011: ldarg.0  			// push arr
				IL_0012: ldloc.1 			// push least
				IL_0013: ldelem.i4 			// push arr[least]
				IL_0014: clt 				// compare arr[j] < arr[least]
				IL_0016: stloc.s 4 			// store comparison
				IL_0018: ldloc.s 4 			// ??
				IL_001a: brfalse.s IL_001e 	// jmp if comparison is false

				IL_001c: ldloc.3 			// push j
				IL_001d: stloc.1 			// least = j (new minimum)

				IL_001e: ldloc.3  			// push j
				IL_001f: ldc.i4.1 			// push 1
				IL_0020: add      			// push j + 1
				IL_0021: stloc.3  			// j = j + 1

				IL_0022: ldloc.3 			// push j (local 3 = j)
				IL_0023: ldarg.0 			// push arr
				IL_0024: ldlen   			// push length of arr?
				IL_0025: conv.i4 			// convert to int?
				IL_0026: clt     			// compare length of arr as int with j
				IL_0028: stloc.s 5 			// store comparison
				IL_002a: ldloc.s 5 			// load comparison (j <  arr.Length)
				IL_002c: brtrue.s IL_000e 	// if comparison true continue (j <  arr.Length)
			// end loop

			IL_002e: ldarg.0 				// push arr
			IL_002f: ldloc.0 				// push i
			IL_0030: ldelem.i4 				// push arr[i]
			IL_0031: stloc.2 				// tmp = arr[i] (local 2)
			IL_0032: ldarg.0 				// push arr
			IL_0033: ldloc.0 				// push i
			IL_0034: ldarg.0 				// push arr
			IL_0035: ldloc.1 				// push least
			IL_0036: ldelem.i4 				// push arr[least]
			IL_0037: stelem.i4 				// arr[i] = arr[least]
			IL_0038: ldarg.0 				// push arr
			IL_0039: ldloc.1 				// push least
			IL_003a: ldloc.2 				// push tmp
			IL_003b: stelem.i4 				// arr[least] = tmp
			IL_003c: nop 					
			IL_003d: ldloc.0 				// push i
			IL_003e: ldc.i4.1 				// push 1
			IL_003f: add      				// push i+1
			IL_0040: stloc.0 				// i = i + 1

			IL_0041: ldloc.0 				// push i
			IL_0042: ldarg.0 				// push arr
			IL_0043: ldlen  				// push arr.len
			IL_0044: conv.i4 				// convert arr.len to int
			IL_0045: clt    				// compare arr.len with i
			IL_0047: stloc.s 6 				// store comparison
			IL_0049: ldloc.s 6 				// load comparison 
			IL_004b: brtrue.s IL_0005 		// continue if comparison (i < arr.Length)
		// end loop

		IL_004d: ret 
	} // end of method Selsort::SelectionSort