using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DamageController))]
public class DamageControllerEditor : Editor
{
    private DamageController objeto;
    private List<string> tags;

    void OnEnable()
    {
        objeto = (DamageController)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // desenha o insector padrão definido no MonoBehaviour

        // Cria um campo para selecionar uma ou mais tags
        objeto.targetTagsMask = EditorGUILayout.MaskField("Target Tags", objeto.targetTagsMask, UnityEditorInternal.InternalEditorUtility.tags);
        tags = new List<string>();

        // Quando algum campo for alterado
        if (GUI.changed)
        {
            // Adiciona as tags selecionadas a uma lista e no final atribui o valor ao script desejado
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; ++i)
            {
                int layer = 1 << i;

                if ((objeto.targetTagsMask & layer) != 0) // a tag na posição i foi selecionada
                {
                    tags.Add(UnityEditorInternal.InternalEditorUtility.tags[i]);
                }
            }

            objeto.targetTags = tags.ToArray();
        }
    }
}
