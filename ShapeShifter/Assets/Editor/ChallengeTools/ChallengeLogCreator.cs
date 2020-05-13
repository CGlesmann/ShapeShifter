using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ChallengeLogCreator : EditorWindow
{
    private ChallengeLog currentChallengeLog = null;
    private int selectedPackIndex = 0;
    private int selectedLevelIndex = 0;

    [MenuItem("ChallengeLog Creator", menuItem = "Global Tools/ChallengeLog Creator")]
    public static void OpenChallengeCreator()
    {
        ChallengeLogCreator logCreator = GetWindow<ChallengeLogCreator>();
        logCreator.Show();
    }

    private void OnGUI()
    {
        /*
        selectedPackIndex = EditorGUILayout.IntField("Pack", selectedPackIndex, GUILayout.MaxWidth(200f));
        selectedLevelIndex = EditorGUILayout.IntField("Level", selectedLevelIndex, GUILayout.MaxWidth(200f));
        */
        if (GameObject.FindObjectOfType<GameManager>() != null)
        {
            string[] levelTitleIndexes = EditorSceneManager.GetActiveScene().name.Split('_')[1].Split('-');
            selectedPackIndex = Int32.Parse(levelTitleIndexes[0]);
            selectedLevelIndex = Int32.Parse(levelTitleIndexes[1]);

            currentChallengeLog = Resources.Load<ChallengeLog>($"ChallengeLogs/Level_Pack_{selectedPackIndex}/Level_{selectedLevelIndex}");
            if (currentChallengeLog == null)
            {
                ChallengeLogCreator window = GetWindow<ChallengeLogCreator>();
                if (GUI.Button(new Rect((window.position.width / 2) - 125f,
                                        (window.position.height / 2) - 30f,
                                        250f,
                                        60f), $"Create Challenge Log For Level {selectedPackIndex}-{selectedLevelIndex}"))
                {
                    currentChallengeLog = CreateChallengeLog(selectedPackIndex, selectedLevelIndex);
                }
            }
            else
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Challenge Log - Level {selectedPackIndex}-{selectedLevelIndex}", EditorStyles.boldLabel, GUILayout.MaxWidth(200f));
                if (GUILayout.Button("Add Challenge", GUILayout.MaxWidth(150f)))
                {
                    List<Type> challengeTypes = GetAllChallengeTypes();
                    GenericMenu challengeDropDown = new GenericMenu();
                    foreach (Type type in challengeTypes)
                    {
                        challengeDropDown.AddItem(new GUIContent($"Add {type.ToString()}"), false, AddChallenge, CastChallenge(type));
                    }
                    challengeDropDown.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Separator();
                GUIStyle style = new GUIStyle();
                style.fontSize = 28;
                style.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField("Challenges", style);
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                for (int i = 0; i < currentChallengeLog.GetChallengeCount(); i++)
                {
                    Challenge data = currentChallengeLog.GetChallengeData(i);

                    DrawBaseChallenge(i, data);
                    EditorGUILayout.Separator();
                }

                if (GUI.changed)
                    EditorUtility.SetDirty(currentChallengeLog);
            }
        } else
        {
            EditorGUILayout.LabelField("Navigate to a level scene to use this editor tool", EditorStyles.centeredGreyMiniLabel);
        }
    }

    #region Challenge Draw Functions
    private void DrawBaseChallenge(int index, Challenge challengeData)
    {
        if (challengeData == null)
            return;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("X", GUILayout.MaxWidth(24)))
        {
            currentChallengeLog.RemoveChallenge(index);
            return;
        }

        GUIStyle challengeHeaderStyle = new GUIStyle();
        challengeHeaderStyle.fontSize = 14;
        challengeHeaderStyle.fontStyle = FontStyle.BoldAndItalic;
        challengeHeaderStyle.normal.textColor = Color.white;

        EditorGUILayout.LabelField($"{challengeData.GetType()}", challengeHeaderStyle, GUILayout.MaxWidth(150f));
        EditorGUILayout.EndHorizontal();
        challengeData.challengeDescription = EditorGUILayout.TextField("Description", challengeData.challengeDescription, GUILayout.MaxWidth(550f));

        // Challenge Specific Fields
        DrawByChallengeType(challengeData);
    }

    private void DrawByChallengeType(Challenge challenge)
    {
        if (challenge.GetType() == typeof(TimeChallenge))
        {
            DrawTimeChallenge((TimeChallenge)challenge);
            return;
        }

        if (challenge.GetType() == typeof(MoveChallenge))
        {
            DrawMoveChallenge(challenge as MoveChallenge);
            return;
        }

        if (challenge.GetType() == typeof(ModeSwitchChallenge))
        {
            DrawModeSwitchChallenge(challenge as ModeSwitchChallenge);
            return;
        }
    }

    private void DrawTimeChallenge(TimeChallenge challengeData)
    {
        if (challengeData == null)
            return;

        EditorGUILayout.Separator();
        challengeData.requiredTime = EditorGUILayout.FloatField("Required Time (Seconds)", challengeData.requiredTime, GUILayout.MaxWidth(225f));
    }

    private void DrawMoveChallenge(MoveChallenge challengeData)
    {
        if (challengeData == null)
            return;

        EditorGUILayout.Separator();
        challengeData.requiredMoves = EditorGUILayout.IntField("Required Moves", challengeData.requiredMoves, GUILayout.MaxWidth(225f));
    }

    private void DrawModeSwitchChallenge(ModeSwitchChallenge challengeData)
    {
        if (challengeData == null)
            return;

        EditorGUILayout.Separator();
        challengeData.maximumModeSwitches = EditorGUILayout.IntField("Maximum Switches", challengeData.maximumModeSwitches, GUILayout.MaxWidth(225f));
    }
    #endregion

    #region Helper Functions
    private ChallengeLog CreateChallengeLog(int packIndex, int levelIndex)
    {
        if (!Directory.Exists($"Assets/Resources/ChallengeLogs/Level_Pack_{packIndex}"))
            Directory.CreateDirectory($"Assets/Resources/ChallengeLogs/Level_Pack_{packIndex}");

        ChallengeLog newChallengeLog = CreateInstance<ChallengeLog>();
        AssetDatabase.CreateAsset(newChallengeLog, $"Assets/Resources/ChallengeLogs/Level_Pack_{packIndex}/Level_{levelIndex}.asset");

        return newChallengeLog;
    }

    private void AddChallenge(object challenge)
    {
        if (challenge == null)
            Debug.LogError("Could not add that challenge type");

        currentChallengeLog.AddChallenge((Challenge)challenge);
    }

    private Challenge CastChallenge(Type challengeType)
    {
        Challenge newChallenge = null;
        if (challengeType == typeof(MoveChallenge))
            newChallenge = new MoveChallenge();

        if (challengeType == typeof(TimeChallenge))
            newChallenge = new TimeChallenge();

        if (challengeType == typeof(ModeSwitchChallenge))
            newChallenge = new ModeSwitchChallenge();

        return newChallenge;
    }

    private List<Type> GetAllChallengeTypes()
    {
        List<Type> challengeTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from assemblyType in domainAssembly.GetTypes()
                                 where typeof(Challenge).IsAssignableFrom(assemblyType)
                                 select assemblyType).ToList<Type>();

        challengeTypes.Remove(typeof(Challenge));
        return challengeTypes;
    }
    #endregion
}
