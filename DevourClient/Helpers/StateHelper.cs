using UnityEngine;
using Il2CppOpsive.UltimateCharacterController.Character;
using System.Collections.Generic;
using System.Collections;
using MelonLoader;
using Il2CppPhoton.Bolt;

namespace DevourClient.Helpers
{
    public class BasePlayer
    {
        public GameObject p_GameObject { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Id { get; set; } = default!;

        public void Kill()
        {
            if (p_GameObject == null)
            {
                return;
            }

            Il2Cpp.SurvivalAzazelBehaviour sab = Il2Cpp.SurvivalAzazelBehaviour.FindObjectOfType<Il2Cpp.SurvivalAzazelBehaviour>();

            if (sab == null)
            {
                return;
            }

            sab.OnKnockout(sab.gameObject, p_GameObject);
        }

        public void Revive()
        {
            if (p_GameObject == null)
            {
                return;
            }

            Il2Cpp.NolanBehaviour nb = p_GameObject.GetComponent<Il2Cpp.NolanBehaviour>();
            Il2Cpp.SurvivalReviveInteractable _reviveInteractable = UnityEngine.Object.FindObjectOfType<Il2Cpp.SurvivalReviveInteractable>();

            _reviveInteractable.Interact(nb.gameObject);
        }

        public void Jumpscare()
        {
            if (!BoltNetwork.IsServer)
            {
                MelonLogger.Msg("You need to be server !");
                Hacks.Misc.ShowMessageBox("You need to be server !");
                return;
            }

            if (p_GameObject == null)
            {
                return;
            }

            Il2Cpp.SurvivalAzazelBehaviour sab = Il2Cpp.SurvivalAzazelBehaviour.FindObjectOfType<Il2Cpp.SurvivalAzazelBehaviour>();

            if (sab == null)
            {
                return;
            }

            sab.OnPickedUpPlayer(sab.gameObject, p_GameObject, false);

            MelonLogger.Msg(Name);
            Hacks.Misc.ShowMessageBox(Name);

            /*
            MelonLogger.Msg(Name);
            Il2Cpp.JumpScare _jumpscare = UnityEngine.Object.FindObjectOfType<Il2Cpp.JumpScare>();
            _jumpscare.player = p_GameObject;
            _jumpscare.Activate(p_GameObject.GetComponent<BoltEntity>());
            */
        }

        public void LockInCage()
        {
            if (p_GameObject == null)
            {
                return;
            }

            BoltNetwork.Instantiate(BoltPrefabs.Cage, p_GameObject.transform.position, Quaternion.identity);
        }

        public void TP()
        {
            if (p_GameObject == null)
            {
                return;
            }

            Il2Cpp.NolanBehaviour nb = Player.GetPlayer();
            nb.TeleportTo(p_GameObject.transform.position, Quaternion.identity);
        }

        public void TPAzazel()
        {
            if (p_GameObject == null)
            {
                return;
            }

            UltimateCharacterLocomotion ucl = Helpers.Map.GetAzazel().GetComponent<UltimateCharacterLocomotion>();

            if (ucl)
            {
                ucl.SetPosition(p_GameObject.transform.position);
            }
            else
            {
                MelonLogger.Error("Azazel not found!");
                Hacks.Misc.ShowMessageBox("Azazel not found!");
                return;
            }
        }

        public void ShootPlayer()
        {
            if (!BoltNetwork.IsServer)
            {
                MelonLogger.Msg("You need to be server !");
                Hacks.Misc.ShowMessageBox("You need to be server !");
                return;
            }

            if (p_GameObject == null)
            {
                return;
            }

            Il2Cpp.AzazelSamBehaviour _azazelSam = UnityEngine.Object.FindObjectOfType<Il2Cpp.AzazelSamBehaviour>();

            if (_azazelSam)
            {
                _azazelSam.OnShootPlayer(p_GameObject, true);
            }
        }
    }
    public class Player
    {
        public static bool IsInGame()
        {
            Il2Cpp.OptionsHelpers optionsHelpers = UnityEngine.Object.FindObjectOfType<Il2Cpp.OptionsHelpers>();
            return optionsHelpers.inGame;
        }

        public static bool IsInGameOrLobby()
        {
            return GetPlayer() != null;
        }

        public static Il2Cpp.NolanBehaviour GetPlayer()
        {
            if (Entities.LocalPlayer_.p_GameObject == null)
            {
                return null!;
            }

            return Entities.LocalPlayer_.p_GameObject.GetComponent<Il2Cpp.NolanBehaviour>();
        }

        public static bool IsPlayerCrawling()
        {
            Il2Cpp.NolanBehaviour nb = Player.GetPlayer();

            if (nb == null)
            {
                return false;
            }

            return nb.IsCrawling();
        }

    }

    public class Entities
    {
        private static bool isRunning = true;  // Control coroutine running state
        private static List<object> activeCoroutines = new List<object>();  // Track active coroutines

        public static int MAX_PLAYERS = 4; //will change by calling CreateCustomizedLobby

        public static BasePlayer LocalPlayer_ = new BasePlayer();
        public static BasePlayer[] Players = default!;
        public static Il2Cpp.GoatBehaviour[] GoatsAndRats = default!;
        public static Il2Cpp.SurvivalInteractable[] SurvivalInteractables = default!;
        public static Il2Cpp.KeyBehaviour[] Keys = default!;
        public static Il2Cpp.SurvivalDemonBehaviour[] Demons = default!;
        public static Il2Cpp.SpiderBehaviour[] Spiders = default!;
        public static Il2Cpp.GhostBehaviour[] Ghosts = default!;
        public static Il2Cpp.SurvivalAzazelBehaviour[] Azazels = default!;
        public static Il2Cpp.BoarBehaviour[] Boars = default!;
        public static Il2Cpp.CorpseBehaviour[] Corpses = default!;
        public static Il2Cpp.CrowBehaviour[] Crows = default!;
        public static Il2Cpp.ManorLumpController[] Lumps = default!;

        // Method to stop all coroutines
        public static void StopAllCoroutines()
        {
            isRunning = false;
            foreach (var coroutine in activeCoroutines)
            {
                if (coroutine != null)
                {
                    MelonCoroutines.Stop(coroutine);
                }
            }
            activeCoroutines.Clear();
            
            // Clean up all cached objects
            CleanupCachedObjects();
        }

        // Clean up cached objects
        private static void CleanupCachedObjects()
        {
            if (Players != null)
            {
                foreach (var player in Players)
                {
                    if (player != null)
                    {
                        player.p_GameObject = null;
                    }
                }
                Players = null;
            }

            LocalPlayer_.p_GameObject = null;
            GoatsAndRats = null;
            SurvivalInteractables = null;
            Keys = null;
            Demons = null;
            Spiders = null;
            Ghosts = null;
            Azazels = null;
            Boars = null;
            Corpses = null;
            Crows = null;
            Lumps = null;
        }

        // Method to start all coroutines
        public static void StartAllCoroutines()
        {
            isRunning = true;
            activeCoroutines.Clear();
            
            // Start all coroutines and save references
            activeCoroutines.Add(MelonCoroutines.Start(GetLocalPlayer()));
            activeCoroutines.Add(MelonCoroutines.Start(GetAllPlayers()));
            activeCoroutines.Add(MelonCoroutines.Start(GetGoatsAndRats()));
            activeCoroutines.Add(MelonCoroutines.Start(GetSurvivalInteractables()));
            activeCoroutines.Add(MelonCoroutines.Start(GetKeys()));
            activeCoroutines.Add(MelonCoroutines.Start(GetDemons()));
            activeCoroutines.Add(MelonCoroutines.Start(GetSpiders()));
            activeCoroutines.Add(MelonCoroutines.Start(GetGhosts()));
            activeCoroutines.Add(MelonCoroutines.Start(GetBoars()));
            activeCoroutines.Add(MelonCoroutines.Start(GetCorpses()));
            activeCoroutines.Add(MelonCoroutines.Start(GetCrows()));
            activeCoroutines.Add(MelonCoroutines.Start(GetLumps()));
            activeCoroutines.Add(MelonCoroutines.Start(GetAzazels()));
        }

        public static IEnumerator GetLocalPlayer()
        {
            while (isRunning)
            {
                try
                {
                    GameObject[] currentPlayers = GameObject.FindGameObjectsWithTag("Player");
                    if (currentPlayers != null)
                    {
                        for (int i = 0; i < currentPlayers.Length; i++)
                        {
                            if (currentPlayers[i].GetComponent<Il2Cpp.NolanBehaviour>().entity.IsOwner)
                            {
                                // Clean up old references before updating
                                if (LocalPlayer_.p_GameObject != currentPlayers[i])
                                {
                                    LocalPlayer_.p_GameObject = currentPlayers[i];
                                }
                                break;
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    string err = $"GetLocalPlayer coroutine encountered an error: {e.Message}";
                    MelonLogger.Error(err);
                    DevourClient.Settings.Settings.errorMessage = err;
                    DevourClient.Settings.Settings.showErrorMessage = true;
                }

                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetAllPlayers()
        {
            while (isRunning)
            {
                try
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    
                    // Clean up old array before creating new one
                    if (Players != null)
                    {
                        foreach (var player in Players)
                        {
                            if (player != null)
                            {
                                player.p_GameObject = null;
                            }
                        }
                    }

                    // Create new array
                    Players = new BasePlayer[players.Length];
                    
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (Players[i] == null)
                        {
                            Players[i] = new BasePlayer();
                        }

                        Players[i].p_GameObject = players[i];
                        
                        Il2Cpp.DissonancePlayerTracking dpt = players[i].GetComponent<Il2Cpp.DissonancePlayerTracking>();
                        if (dpt != null)
                        {
                            Players[i].Name = dpt.state.PlayerName;
                            Players[i].Id = dpt.state.PlayerId;
                        }
                    }
                }
                catch (System.Exception e)
                {
                    string err = $"GetAllPlayers coroutine encountered an error: {e.Message}";
                    MelonLogger.Error(err);
                    DevourClient.Settings.Settings.errorMessage = err;
                    DevourClient.Settings.Settings.showErrorMessage = true;
                }

                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetGoatsAndRats()
        {
            while (isRunning)
            {
                try
                {
                    GoatsAndRats = Il2Cpp.GoatBehaviour.FindObjectsOfType<Il2Cpp.GoatBehaviour>();
                }
                catch (System.Exception e)
                {
                    string err = $"GetGoatsAndRats coroutine encountered an error: {e.Message}";
                    MelonLogger.Error(err);
                    DevourClient.Settings.Settings.errorMessage = err;
                    DevourClient.Settings.Settings.showErrorMessage = true;
                }

                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetSurvivalInteractables()
        {
            while (isRunning)
            {
                try
                {
                    SurvivalInteractables = Il2Cpp.SurvivalInteractable.FindObjectsOfType<Il2Cpp.SurvivalInteractable>();
                }
                catch (System.Exception e)
                {
                    string err = $"GetSurvivalInteractables coroutine encountered an error: {e.Message}";
                    MelonLogger.Error(err);
                    DevourClient.Settings.Settings.errorMessage = err;
                    DevourClient.Settings.Settings.showErrorMessage = true;
                }

                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetKeys()
        {
            while (isRunning)
            {
                try
                {
                    Keys = Il2Cpp.KeyBehaviour.FindObjectsOfType<Il2Cpp.KeyBehaviour>();
                }
                catch (System.Exception e)
                {
                    string err = $"GetKeys coroutine encountered an error: {e.Message}";
                    MelonLogger.Error(err);
                    DevourClient.Settings.Settings.errorMessage = err;
                    DevourClient.Settings.Settings.showErrorMessage = true;
                }

                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetDemons()
        {
            while (isRunning)
            {
                try
                {
                    Demons = Il2Cpp.SurvivalDemonBehaviour.FindObjectsOfType<Il2Cpp.SurvivalDemonBehaviour>();
                }
                catch (System.Exception e)
                {
                    string err = $"GetDemons coroutine encountered an error: {e.Message}";
                    MelonLogger.Error(err);
                    DevourClient.Settings.Settings.errorMessage = err;
                    DevourClient.Settings.Settings.showErrorMessage = true;
                }

                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetSpiders()
        {
            while (isRunning)
            {
                try
                {
                    Spiders = Il2Cpp.SpiderBehaviour.FindObjectsOfType<Il2Cpp.SpiderBehaviour>();
                }
                catch (System.Exception e)
                {
                    string err = $"GetSpiders coroutine encountered an error: {e.Message}";
                    MelonLogger.Error(err);
                    DevourClient.Settings.Settings.errorMessage = err;
                    DevourClient.Settings.Settings.showErrorMessage = true;
                }

                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetGhosts()
        {
            while (isRunning)
            {
                try
                {
                    Ghosts = Il2Cpp.GhostBehaviour.FindObjectsOfType<Il2Cpp.GhostBehaviour>();
                }
                catch (System.Exception e)
                {
                    string err = $"GetGhosts coroutine encountered an error: {e.Message}";
                    MelonLogger.Error(err);
                    DevourClient.Settings.Settings.errorMessage = err;
                    DevourClient.Settings.Settings.showErrorMessage = true;
                }

                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetBoars()
        {
            while (isRunning)
            {
                try
                {
                    Boars = Il2Cpp.BoarBehaviour.FindObjectsOfType<Il2Cpp.BoarBehaviour>();
                }
                catch (System.Exception e)
                {
                    string err = $"GetBoars coroutine encountered an error: {e.Message}";
                    MelonLogger.Error(err);
                    DevourClient.Settings.Settings.errorMessage = err;
                    DevourClient.Settings.Settings.showErrorMessage = true;
                }

                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetCorpses()
        {
            while (isRunning)
            {
                try
                {
                    Corpses = Il2Cpp.CorpseBehaviour.FindObjectsOfType<Il2Cpp.CorpseBehaviour>();
                }
                catch (System.Exception e)
                {
                    string err = $"GetCorpses coroutine encountered an error: {e.Message}";
                    MelonLogger.Error(err);
                    DevourClient.Settings.Settings.errorMessage = err;
                    DevourClient.Settings.Settings.showErrorMessage = true;
                }

                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetCrows()
        {
            while (isRunning)
            {
                try
                {
                    Crows = Il2Cpp.CrowBehaviour.FindObjectsOfType<Il2Cpp.CrowBehaviour>();
                }
                catch (System.Exception e)
                {
                    string err = $"GetCrows coroutine encountered an error: {e.Message}";
                    MelonLogger.Error(err);
                    DevourClient.Settings.Settings.errorMessage = err;
                    DevourClient.Settings.Settings.showErrorMessage = true;
                }

                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetLumps()
        {
            while (isRunning)
            {
                try
                {
                    Lumps = Il2Cpp.ManorLumpController.FindObjectsOfType<Il2Cpp.ManorLumpController>();
                }
                catch (System.Exception e)
                {
                    string err = $"GetLumps coroutine encountered an error: {e.Message}";
                    MelonLogger.Error(err);
                    DevourClient.Settings.Settings.errorMessage = err;
                    DevourClient.Settings.Settings.showErrorMessage = true;
                }

                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetAzazels()
        {
            while (isRunning)
            {
                try
                {
                    Azazels = Il2Cpp.SurvivalAzazelBehaviour.FindObjectsOfType<Il2Cpp.SurvivalAzazelBehaviour>();
                }
                catch (System.Exception e)
                {
                    string err = $"Error in GetAzazels coroutine: {e.Message}";
                    MelonLogger.Error(err);
                    DevourClient.Settings.Settings.errorMessage = err;
                    DevourClient.Settings.Settings.showErrorMessage = true;
                }

                yield return new WaitForSeconds(5f);
            }
        }
    }
}
