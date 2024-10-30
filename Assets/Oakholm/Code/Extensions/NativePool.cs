using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Oakholm.Extensions {

	[StructLayout(LayoutKind.Sequential)]
	[NativeContainer]
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new[] { typeof(int) })]
	public struct NativePool<T> where T : unmanaged, IEquatable<T> {

		private NativeList<T> pool;

		public NativePool(AllocatorManager.AllocatorHandle allocator) {
			pool = new NativeList<T>(0, allocator);
		}

		public T Get() {
			if (pool.Length <= 0) {
				return default(T);
			}
			T obj = pool[0];
			pool.RemoveAt(0);
			pool.Capacity -= 1;
			return obj;
		}

		public void Release(T obj) {
			if (pool.Capacity < pool.Length + 1) {
				pool.Capacity += 1;
			}
			pool[^1] = obj;
		}

		public void Dispose() {
			pool.Dispose();
		}
	}
}
