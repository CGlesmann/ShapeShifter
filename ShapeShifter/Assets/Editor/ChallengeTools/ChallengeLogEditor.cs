using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChallengeLogEditor : EditorWindow
{
    public static ChallengeLog challengeLog => Resources.Load<ChallengeLog>("ChallengeLog/Challenge Log");
    public static List<bool> challengeFoldouts = new List<bool>();

    public static Vector2 scrollPos;
    public static int selectedLevel = -1;

    [MenuItem("Challenge Log Editor", menuItem = "Global Tools/Challenge Log Editor")]
    public static void OpenLogEditor()
    {
        ChallengeLogEditor logEditor = CreateWindow<ChallengeLogEditor>();
        logEditor.Show();
    }

    private void OnGUI()
    {
        GUIStyle unselectedLevelButtonStyle = new GUIStyle();
        unselectedLevelButtonStyle.normal.background = EditorGUIUtility.Load("ChallengeLogEditor/BoxImage.png") as Texture2D;
        unselectedLevelButtonStyle.fontSize = 16;
        unselectedLevelButtonStyle.fontStyle = FontStyle.Bold;
        unselectedLevelButtonStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle selectedLevelButtonStyle = new GUIStyle();
        selectedLevelButtonStyle.normal.background = EditorGUIUtility.Load("ChallengeLogEditor/BoxImage_Selected.png") as Texture2D;
        selectedLevelButtonStyle.fontSize = 16;
        selectedLevelButtonStyle.fontStyle = FontStyle.Bold;
        selectedLevelButtonStyle.alignment = TextAnchor.MiddleCenter;

        if (GUILayout.Button("Add Level List", GUILayout.Width(250f)))
        {
            challengeLog.challengeLog.Add(new ChallengeList());
            EditorUtility.SetDirty(challengeLog);
        }

        EditorGUILayout.BeginHorizontal();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(250f));
        for(int i = 0; i < challengeLog.challengeLog.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(string.Format("Level {0} ({1})", i + 1, challengeLog.challengeLog[i].challenges.Count), selectedLevel == i ? selectedLevelButtonStyle : unselectedLevelButtonStyle, GUILayout.Width(208f), GUILayout.Height(32f)))
            {
                if (selectedLevel != i)
                {
                    selectedLevel = i;

                    challengeFoldouts = new List<bool>();
                    if (challengeLog.challengeLog[i].challenges.Count > 0)
                    {
                        for (int j = 0; j < challengeLog.challengeLog[i].challenges.Count; j++)
                            challengeFoldouts.Add(false);
                    }
                }
                else
                    selectedLevel = -1;
            }

            if (GUILayout.Button("-", selectedLevel == i ? unselectedLevelButtonStyle : selectedLevelButtonStyle, GUILayout.Height(32f)))
            {
                challengeLog.challengeLog.RemoveAt(i);
                EditorUtility.SetDirty(challengeLog);

                continue;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8f);
        }
        EditorGUILayout.EndScrollView();

        if (selectedLevel != -1)
        {
            EditorGUILayout.BeginVertical();
            GUIStyle header = new GUIStyle();
            header.normal.background = EditorGUIUtility.Load("ChallengeLogEditor/BoxImage.png") as Texture2D;
            header.fontSize = 28;
            header.fontStyle = FontStyle.Bold;
            header.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.LabelField(string.Format("Level {0} Challenges", selectedLevel + 1), header, GUILayout.Height(32f));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Time Challenge", selectedLevelButtonStyle, GUILayout.Width(200f), GUILayout.Height(24f)))
            {
                challengeLog.challengeLog[selectedLevel].challenges.Add(new TimeChallenge());
                challengeFoldouts.Add(false);
            }
            if (GUILayout.Button("Add Move Challenge", selectedLevelButtonStyle, GUILayout.Width(200f), GUILayout.Height(24f)))
            {
                challengeLog.challengeLog[selectedLevel].challenges.Add(new MoveChallenge());
                challengeFoldouts.Add(false);
            }
            EditorGUILayout.EndHorizontal();

            if (challengeLog.challengeLog[selectedLevel].challenges.Count > 0)
            {
                EditorGUILayout.Separator();

                Challenge challenge;
                for (int k = 0; k < challengeLog.challengeLog[selectedLevel].challenges.Count; k++)
                {
                    challenge = challengeLog.challengeLog[selectedLevel].challenges[k] as Challenge;
                    challengeFoldouts[k] = EditorGUILayout.BeginFoldoutHeaderGroup(challengeFoldouts[k], string.Format("Challenge {0} ({1})", k + 1, challenge?.GetType()));
                    if (challengeFoldouts[k])
                    {
                        if (GUILayout.Button("Remove", GUILayout.Width(150f)))
                        {
                            challengeLog.challengeLog[selectedLevel].challenges.RemoveAt(k);
                            challengeFoldouts.RemoveAt(k);

                            continue;
                        }

                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField("Base Challenge Fields", EditorStyles.boldLabel);
                        challenge.challengeDescription = EditorGUILayout.TextField("Challenge Description", challenge.challengeDescription, GUILayout.Width(550f));

                        EditorGUILayout.Separator();
                        if (challenge.GetType() == typeof(TimeChallenge))
                            DrawTimeChallenge(challenge as TimeChallenge);
                        if (challenge.GetType() == typeof(MoveChallenge))
                            DrawMoveChallenge(challenge as MoveChallenge);
                    }

                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.Separator();
                }
            }
            EditorGUILayout.EndVertical();
        }
        else
            EditorGUILayout.LabelField("Select a level from the list on the left", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.EndHorizontal();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(challengeLog);
        }
    }

    public void DrawTimeChallenge(TimeChallenge timeChallenge)
    {
        EditorGUILayout.LabelField("Time Challenge Fields", EditorStyles.boldLabel);
        timeChallenge.requiredTime = EditorGUILayout.FloatField("Required Time (Seconds)", timeChallenge.requiredTime, GUILayout.Width(300f));
    }

    public void DrawMoveChallenge(MoveChallenge moveChallenge)
    {
        EditorGUILayout.LabelField("Move Challenge Fields", EditorStyles.boldLabel);
        moveChallenge.requiredMoves = EditorGUILayout.IntField("Required Moves", moveChallenge.requiredMoves, GUILayout.Width(300f));
    }
}
