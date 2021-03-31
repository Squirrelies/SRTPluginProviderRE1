using ProcessMemory;
using System;
using System.Diagnostics;

namespace SRTPluginProviderRE1
{
    internal class GameMemoryRE1Scanner : IDisposable
    {
        // Variables
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryRE1 gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public int ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;

        // Pointer Address Variables
        private int pointerGameState;

        // Pointer Classes
        private IntPtr BaseAddress { get; set; }
        private MultilevelPointer PointerGameState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proc"></param>
        internal GameMemoryRE1Scanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryRE1();
            if (process != null)
                Initialize(process);
        }

        internal void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            if (!SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName)))
                return; // Unknown version.

            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler(pid);
            if (ProcessRunning)
            {
                BaseAddress = NativeWrappers.GetProcessBaseAddress(pid, PInvoke.ListModules.LIST_MODULES_32BIT); // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.
                
                // Setup the pointers.
                PointerGameState = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerGameState));
            }
        }

        private bool SelectPointerAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.REmake_Latest:
                    {
                        pointerGameState = 0x0097C9C0;

                        return true;
                    }
            }

            // If we made it this far... rest in pepperonis. We have failed to detect any of the correct versions we support and have no idea what pointer addresses to use. Bail out.
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdatePointers()
        {
            PointerGameState.UpdatePointers();
        }

        internal unsafe IGameMemoryRE1 Refresh()
        {
            fixed (int* p = &gameMemoryValues._mGameMode)
                PointerGameState.TryDerefInt(0x20, p);

            fixed (int* p = &gameMemoryValues._mDifficulty)
                PointerGameState.TryDerefInt(0x24, p);

            fixed (int* p = &gameMemoryValues._mStartPlayer)
                PointerGameState.TryDerefInt(0x5110, p);

            fixed (int* p = &gameMemoryValues._mCostumeID)
                PointerGameState.TryDerefInt(0x5114, p);

            fixed (int* p = &gameMemoryValues._mChangePlayerID)
                PointerGameState.TryDerefInt(0x5118, p);

            fixed (int* p = &gameMemoryValues._mDisplayMode)
                PointerGameState.TryDerefInt(0x511C, p);

            fixed (int* p = &gameMemoryValues._mVoiceType)
                PointerGameState.TryDerefInt(0x5120, p);

            fixed (int* p = &gameMemoryValues._mShadowQuality)
                PointerGameState.TryDerefInt(0x5104, p);

            fixed (int* p = &gameMemoryValues._mIsSubWepAuto)
                PointerGameState.TryDerefInt(0x5128, p);

            fixed (float* p = &gameMemoryValues._mFrameCounter)
                PointerGameState.TryDerefFloat(0xE4738, p);

            fixed (float* p = &gameMemoryValues._mPlayTime)
                PointerGameState.TryDerefFloat(0xE474C, p);

            fixed (byte* p = &gameMemoryValues._mIsStartGame)
                PointerGameState.TryDerefByte(0xE477E, p);

            fixed (byte* p = &gameMemoryValues._mIsLoadGame)
                PointerGameState.TryDerefByte(0xE477C, p);

            if (gameMemoryValues._inventory == null)
            {
                gameMemoryValues._inventory = new InventoryEntry[10];
                for (int i = 0; i < gameMemoryValues._inventory.Length; ++i)
                    gameMemoryValues._inventory[i] = new InventoryEntry() { _data = new int[2], SlotPosition = i, IsEquipped = (gameMemoryValues.mStartPlayer == CharacterEnumeration.Jill && i == 9) || ((gameMemoryValues.mStartPlayer == CharacterEnumeration.Chris || gameMemoryValues.mStartPlayer == CharacterEnumeration.Rebecca) && i == 7) };
            }
            for (int i = 0; i < ((gameMemoryValues.mStartPlayer == CharacterEnumeration.Jill) ? 10 : 8); ++i) // Ten (10) for Jill, eight (8) for Chris and Rebecca. Characters have two (2) extra slots for storing the special item (lock pick or lighter) and equipped weapon information.
            {
                fixed (int* p = &gameMemoryValues._inventory[i]._data[0])
                {
                    PointerGameState.TryDerefInt(0x38 + (i * 8), p);
                    PointerGameState.TryDerefInt(0x38 + (i * 8) + 4, p + 1);
                }
            }

            HasScanned = true;
            return gameMemoryValues;
        }

        private int? GetProcessId(Process process) => process?.Id;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (memoryAccess != null)
                        memoryAccess.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~REmake1Memory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}