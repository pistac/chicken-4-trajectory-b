using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Animations;
using System.Runtime.InteropServices;

namespace MesoGames.CuztomChar
{
    public class CuztomCharEditor : EditorWindow
    {
        [MenuItem( "Assets/Meso Games Utils/Character Cuztomizer %g" )]
        static void ShowWindow()
        {
            _thisWindow = EditorWindow.GetWindow( typeof( CuztomCharEditor ), false, "Cuztomizer", true );
            _thisWindow.ShowTab();
            _thisWindow.ShowUtility();
        }

        protected void Update()
        {
            if( _thisEditor != null )
            {
                Repaint();
                _thisEditor.ReloadPreviewInstances();
            }
        }

        protected void OnEnable()
        {
            LoadModelProperties();
        }

        protected void OnDestroy()
        {
            ResetModelProperties();
        }

        protected void OnGUI()
        {
            if( arrModelProperties.Count == 0 )
            {
                return;
            }

            // Gender selection
            selectedBase = arrModelProperties[ selectedBaseIdx ];
            if( selectedBase != null )
            {
                if( selectedBase.refCharGo != null )
                {
                    if( ( _thisEditor == null ) | ( isSelectedBaseChanged ) )
                    {
                        isSelectedBaseChanged = false;
                        _thisEditor = Editor.CreateEditor( selectedBase.refCharGo );
                    }
                    selectedBase.ShowDefaults();

                    // PREVIEW
                    AddSeparator();
                    {
                        GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 5 ) );
                        EditorGUILayout.LabelField( "PREVIEW", EditorStyles.boldLabel );
                        AddSeparator();
                        {
                            _thisEditor.OnInteractivePreviewGUI( GUILayoutUtility.GetRect( 320, 320 ), GUIStyle.none );
                        }
                    }

                    AddSeparator();
                    {
                        _newInstanceName = EditorGUILayout.TextField( "New instance name: ", _newInstanceName );

                        EditorGUILayout.BeginHorizontal();
                        {
                            if( GUILayout.Button( "CREATE INSTANCE" ) )
                            {
                                CreateModelInstance();
                            }
                            if( GUILayout.Button( "Reset" ) )
                            {
                                ResetModelProperties();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    // FEATURES
                    AddSeparator( 2 );
                    {
                        GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 5 ) );
                        EditorGUILayout.LabelField( "FEATURES", EditorStyles.boldLabel );

                        AddSeparator();
                        {
                            int newBaseIdx = selectedBaseIdx;
                            newBaseIdx = EditorGUILayout.IntPopup( "Character", newBaseIdx, arrBaseNames.ToArray(), arrBaseIndices.ToArray() );
                            if( newBaseIdx != selectedBaseIdx )
                            {
                                selectedBaseIdx = newBaseIdx;
                                isSelectedBaseChanged = true;
                                return;
                            }
                        }

                        AddSeparator();
                        {
                            SetSkinSelectionGrid( "Skin Color", ref selectedBase.selectedSkinIdx, ref selectedBase.skinMaterials, 3 );
                        }

                        AddSeparator();
                        {
                            SetEnableSlider( "Hairstyle", ref selectedBase.addHair, ref selectedBase.selectedHairIdx, ref selectedBase.baseHairsTx );
                            if( !selectedBase.gender.Contains( "Female" ) )
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    selectedBase.isBald = EditorGUILayout.Toggle( " ", selectedBase.isBald );
                                    EditorGUILayout.LabelField( "is Bald?" );
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            AddSeparator();

                            SetEnableSlider( "Eyebrows", ref selectedBase.addBrows, ref selectedBase.selectedBrowsIdx, ref selectedBase.browsSetsTx );
                            SetSlider( "Eyes", ref selectedBase.selectedEyesIdx, ref selectedBase.eyesSetsTx );
                            SetSlider( "Mouth", ref selectedBase.selectedMouthIdx, ref selectedBase.mouthSetsTx );
                        }

                        AddSeparator();
                        {
                            SetEnableSlider( "Top Clothing",
                                ref selectedBase.addTopClothing,
                                ref selectedBase.selectedTopIdx,
                                ref selectedBase.baseTopsTx );

                            if( selectedBase.gender.Contains( "Female" ) )
                            {
                                if( !selectedBase.addTopClothing )
                                {
                                    selectedBase.addBottomClothing = false;
                                    selectedBase.selectedBottomIdx = 0;
                                }

                                EditorGUI.BeginDisabledGroup( !selectedBase.addTopClothing );
                                {
                                    SetEnableSlider( "Bottom Clothing",
                                        ref selectedBase.addBottomClothing,
                                        ref selectedBase.selectedBottomIdx,
                                        ref selectedBase.baseBottomsTx
                                    );
                                }
                                EditorGUI.EndDisabledGroup();
                            }
                            else
                            {
                                SetEnableSlider( "Bottom Clothing",
                                    ref selectedBase.addBottomClothing,
                                    ref selectedBase.selectedBottomIdx,
                                    ref selectedBase.baseBottomsTx
                                );
                            }

                            if( !selectedBase.addBottomClothing )
                            {
                                selectedBase.addShoes = false;
                                selectedBase.selectedShoesIdx = 0;
                            }

                            EditorGUI.BeginDisabledGroup( !selectedBase.addBottomClothing );
                            {
                                SetEnableSlider( "Shoes",
                                    ref selectedBase.addShoes,
                                    ref selectedBase.selectedShoesIdx,
                                    ref selectedBase.baseShoesTx
                                );
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                    }

                    if( selectedBase.gender.Contains( "Male" ) )
                    {
                        DisplayMalePresetBody();
                    }
                    else
                    {
                        DisplayFemalePresetBody();
                    }
                }
            }
        }

        protected void AddSeparator( int pCount = 1 )
        {
            for( int i = 0; i < pCount; i++ )
            {
                EditorGUILayout.Separator();
            }
        }

        #pragma warning disable
        float tempF = 0.0f;
        #pragma warning restore

        protected void SetSlider( string pLabel, ref int pSliderIdx, ref List< Transform > pSliderValues, int pSliderIdxMin = 0 )
        {
            int count = pSliderValues.Count;

            pSliderIdx = EditorGUILayout.IntSlider( pLabel, pSliderIdx, pSliderIdxMin, ( count - 1 ) );
            for( int i = 0; i < count; i++ )
            {
                pSliderValues[ i ].gameObject.SetActive( false );
                if( pSliderIdx > ( -1 ) )
                {
                    if( i == ( pSliderIdx ) )
                    {
                        pSliderValues[ i ].gameObject.SetActive( true );
                    }
                }
            }
        }

        protected void SetToggleSlider( string pLabel, ref bool pToggle, ref int pSliderIdx, ref List< Transform > pSliderValues, int pSliderIdxMin = 0 )
        {
            pToggle = EditorGUILayout.ToggleLeft( pLabel, pToggle );
            int count = pSliderValues.Count;

            if( pToggle )
            {
                if( pSliderIdx < 0 )
                {
                    pSliderIdx = 0;
                }
                pSliderIdx = EditorGUILayout.IntSlider( "", pSliderIdx, pSliderIdxMin, ( count - 1 ) );
            }
            else
            {
                pSliderIdx = -1;
            }

            for( int i = 0; i < count; i++ )
            {
                pSliderValues[ i ].gameObject.SetActive( false );
                if( pSliderIdx > ( -1 ) )
                {
                    if( i == pSliderIdx )
                    {
                        pSliderValues[ i ].gameObject.SetActive( true );
                    }
                }
            }
        }

        protected void SetEnableSlider( string pLabel, ref bool pToggle, ref int pSliderIdx, ref List< Transform > pSliderValues )
        {
            int count = pSliderValues.Count;
            EditorGUILayout.BeginHorizontal();
            {
                pToggle = EditorGUILayout.Toggle( pLabel, pToggle );

                if( pToggle )
                {
                    if( pSliderIdx < 0 )
                    {
                        pSliderIdx = 0;
                    }
                }

                EditorGUI.BeginDisabledGroup( !pToggle );
                {
                    pSliderIdx = EditorGUILayout.IntSlider( "", pSliderIdx, 0, ( count - 1 ) );
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            for( int i = 0; i < count; i++ )
            {
                pSliderValues[ i ].gameObject.SetActive( false );
                if( pToggle )
                {
                    if( pSliderIdx > ( -1 ) )
                    {
                        if( i == pSliderIdx )
                        {
                            pSliderValues[ i ].gameObject.SetActive( true );
                        }
                    }
                }
            }
        }

        protected void SetToggleSelectionGrid( string pLabel, ref bool pToggle, ref int pGridIdx, ref List< Transform > pGridValues, int pGridColumnCount )
        {
            int gridItems = pGridValues.Count;

            EditorGUILayout.BeginHorizontal();
            {
                pToggle = EditorGUILayout.Toggle( pLabel, pToggle );
                if( pToggle )
                {
                    string[] gridNames = new string[ gridItems ];
                    for( int i = 0; i < gridItems; i++ )
                    {
                        gridNames[ i ] = "" + i;
                    }

                    if( pGridIdx < 0 )
                    {
                        pGridIdx = 0;
                    }
                    pGridIdx = GUILayout.SelectionGrid( pGridIdx, gridNames, pGridColumnCount );
                }
                else
                {
                    pGridIdx = -1;
                }
            }
            EditorGUILayout.EndHorizontal();

            for( int i = 0; i < gridItems; i++ )
            {
                pGridValues[ i ].gameObject.SetActive( false );
                if( pGridIdx > ( -1 ) )
                {
                    if( i == pGridIdx )
                    {
                        pGridValues[ i ].gameObject.SetActive( true );
                    }
                }
            }
        }

        protected void SetSkinSelectionGrid( string pLabel, ref int pGridIdx, ref List< ModelMaterialProperty > pGridValues, int pGridColumns )
        {
            int count = pGridValues.Count;
            string[] labels = new string[ count ];
            for( int i = 0; i < count; i++ )
            {
                labels[ i ] = pGridValues[ i ].displayName;
            }

            if( labels.Length > 0 )
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField( pLabel, EditorStyles.wordWrappedLabel );
                    pGridIdx = GUILayout.SelectionGrid( pGridIdx, labels, pGridColumns, EditorStyles.miniButton );
                }
                EditorGUILayout.EndHorizontal();

                Transform bodyTx = selectedBase.baseBodiesTx.Find( o => ( o.gameObject.activeSelf == true ) );
                if( bodyTx != null )
                {
                    bodyTx.GetComponent< SkinnedMeshRenderer >().material = pGridValues[ pGridIdx ].asset;

                    int scalpCtr = bodyTx.childCount;
                    if( scalpCtr > 0 )
                    {
                        for( int x = 0; x < scalpCtr; x++ )
                        {
                            bodyTx.GetChild( x ).GetComponent< SkinnedMeshRenderer >().material = pGridValues[ pGridIdx ].asset;
                        }
                    }
                }
            }
        }

        private void DisplayMalePresetBody()
        {
            bool hasTop = selectedBase.addTopClothing;
            bool hasBottom = selectedBase.addBottomClothing;
            bool hasShoes = selectedBase.addShoes;

            int count = selectedBase.baseBodiesTx.Count;
            int idx = 0;

            if( hasTop )
            {
                if( hasBottom )
                {
                    if( selectedBase.baseTopsTx[ selectedBase.selectedTopIdx ].name.Contains( "sweater" )
                        | selectedBase.baseTopsTx[ selectedBase.selectedTopIdx ].name.Contains( "suitlong" ) )
                    {
                        if( selectedBase.baseBottomsTx[ selectedBase.selectedBottomIdx ].name.Contains( "short" ) )
                        {
                            idx = 6;    // opt6_longsleeves_shorts
                            if( hasShoes & !( selectedBase.baseShoesTx[ selectedBase.selectedShoesIdx ].name.Contains( "flipflops" ) ) )
                            {
                                idx = 9;    // opt10_longsleeves_shorts_shoes
                            }
                        }
                        else
                        {
                            idx = 7;    // opt7_longsleeves_pants
                            if( hasShoes & !( selectedBase.baseShoesTx[ selectedBase.selectedShoesIdx ].name.Contains( "flipflops" ) ) )
                            {
                                idx = 10;    // opt11_longsleeves_pants_shoes
                            }
                        }
                    }
                    else
                    {
                        if( selectedBase.baseBottomsTx[ selectedBase.selectedBottomIdx ].name.Contains( "short" ) )
                        {
                            idx = 4;    // opt5_tshirt_shorts
                            if( hasShoes & !( selectedBase.baseShoesTx[ selectedBase.selectedShoesIdx ].name.Contains( "flipflops" ) ) )
                            {
                                idx = 8;    // opt8_tshirt_shorts_shoes
                            }
                        }
                        else
                        {
                            idx = 5;    // opt5_tshirt_pants
                            if( hasShoes & !( selectedBase.baseShoesTx[ selectedBase.selectedShoesIdx ].name.Contains( "flipflops" ) ) )
                            {
                                idx = 8;    // opt8_tshirt_shorts_shoes
                            }
                        }
                    }
                }
                else
                {
                    if( selectedBase.baseTopsTx[ selectedBase.selectedTopIdx ].name.Contains( "sweater" )
                        | selectedBase.baseTopsTx[ selectedBase.selectedTopIdx ].name.Contains( "suitlong" ) )
                    {
                        idx = 3;    // opt4_longsleeve
                    }
                    else
                    {
                        idx = 2;    // opt3_tshirt
                    }
                }
            }
            else if( hasBottom )
            {
                if( hasTop )
                {
                    if( selectedBase.baseTopsTx[ selectedBase.selectedTopIdx ].name.Contains( "sweater" )
                        | selectedBase.baseTopsTx[ selectedBase.selectedTopIdx ].name.Contains( "suit" ) )
                    {
                        if( selectedBase.baseBottomsTx[ selectedBase.selectedBottomIdx ].name.Contains( "short" ) )
                        {
                            idx = 6;    // opt6_longsleeves_shorts
                            if( hasShoes & !( selectedBase.baseShoesTx[ selectedBase.selectedShoesIdx ].name.Contains( "flipflops" ) ) )
                            {
                                idx = 9;    // opt10_longsleeves_shorts_shoes
                            }
                        }
                        else
                        {
                            idx = 7;    // opt7_longsleeves_pants
                            if( hasShoes & !( selectedBase.baseShoesTx[ selectedBase.selectedShoesIdx ].name.Contains( "flipflops" ) ) )
                            {
                                idx = 10;    // opt11_longsleeves_pants_shoes
                            }
                        }
                    }
                    else
                    {
                        if( selectedBase.baseBottomsTx[ selectedBase.selectedBottomIdx ].name.Contains( "short" ) )
                        {
                            idx = 4;    // opt5_tshirt_shorts
                            if( hasShoes & !( selectedBase.baseShoesTx[ selectedBase.selectedShoesIdx ].name.Contains( "flipflops" ) ) )
                            {
                                idx = 8;    // opt8_tshirt_shorts_shoes
                            }
                        }
                        else
                        {
                            idx = 5;    // opt5_tshirt_pants
                            if( hasShoes & !( selectedBase.baseShoesTx[ selectedBase.selectedShoesIdx ].name.Contains( "flipflops" ) ) )
                            {
                                idx = 8;    // opt8_tshirt_shorts_shoes
                            }
                        }
                    }
                }
                else
                {
                    if( selectedBase.baseBottomsTx[ selectedBase.selectedBottomIdx ].name.Contains( "short" ) )
                    {
                        idx = 11;    // opt12_shorts
                        if( hasShoes
                            & !( selectedBase.baseShoesTx[ selectedBase.selectedShoesIdx ].name.Contains( "flipflops" ) ) )
                        {
                            idx = 12;    // opt13_shorts_shoes
                        }
                    }
                    else
                    {
                        idx = 13;    // opt14_pants
                        if( hasShoes
                            & !( selectedBase.baseShoesTx[ selectedBase.selectedShoesIdx ].name.Contains( "flipflops" ) ) )
                        {
                            idx = 14;    // opt15_pants_shoes
                        }
                    }
                }
            }
            else
            {
                // default body without any clothing
                idx = 1;
            }

            selectedBase.selectedBodyIdx = idx;

            for( int i = 0; i < count; i++ )
            {
                selectedBase.baseBodiesTx[ i ].gameObject.SetActive( false );
                if( selectedBase.selectedBodyIdx > ( -1 ) )
                {
                    if( i == ( selectedBase.selectedBodyIdx ) )
                    {
                        selectedBase.baseBodiesTx[ i ].gameObject.SetActive( true );

                        int scalpCtr = selectedBase.baseBodiesTx[ i ].childCount;
                        if( scalpCtr > 0 )
                        {
                            for( int x = 0; x < scalpCtr; x++ )
                            {
                                if( selectedBase.baseBodiesTx[ i ].GetChild( x ).name.Contains( "bald" ) )
                                {
                                    selectedBase.baseBodiesTx[ i ].GetChild( x ).gameObject.SetActive( selectedBase.isBald );
                                }
                                else
                                {
                                    selectedBase.baseBodiesTx[ i ].GetChild( x ).gameObject.SetActive( !selectedBase.isBald );
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DisplayFemalePresetBody()
        {
            bool hasTop = selectedBase.addTopClothing;
            bool hasBottom = selectedBase.addBottomClothing;
            bool hasShoes = selectedBase.addShoes;

            int count = selectedBase.baseBodiesTx.Count;
            int idx = 0;

            string refName = "";
            string[] splitName = null;
            string rawName = "";
            string selName = "";

            if( hasTop )
            {
                // Get the selected Top's name
                refName += selectedBase.baseTopsTx[ selectedBase.selectedTopIdx ].name;
                splitName = refName.Split( '_' );

                selName += splitName[ splitName.Length - 1 ];
                if( hasBottom )
                {
                    //Top and Bottom
                    refName += selectedBase.baseBottomsTx[ selectedBase.selectedBottomIdx ].name;
                    splitName = refName.Split( '_' );

                    rawName = splitName[ splitName.Length - 1 ];
                    selName += "_and_pants" + ( rawName.First().ToString().ToUpper() + rawName.Substring( 1 ) );

                    if( hasShoes )
                    {
                        //Top and Bottom and Shoes
                        selName += "Shoes";
                    }

                    idx = selectedBase.baseBodiesTx.IndexOf( selectedBase.baseBodiesTx.Find( o => o.name.Contains( selName ) ) );
                }
                else
                {
                    // Top Only
                    selName = "topOnly_" + selName;
                    idx = selectedBase.baseBodiesTx.IndexOf( selectedBase.baseBodiesTx.Find( o => o.name.Contains( selName ) ) );
                }
            }
            else
            {
                // default body without any clothing
                idx = 0;
            }

            selectedBase.baseBodiesTx[ selectedBase.selectedBodyIdx ].gameObject.SetActive( false );
            selectedBase.selectedBodyIdx = idx;
            selectedBase.baseBodiesTx[ selectedBase.selectedBodyIdx ].gameObject.SetActive( true );
        }

        private void LoadModelProperties()
        {
            string[] modelsPath = System.IO.Directory.GetDirectories( MODELS_ROOTPATH );
            string[] materialsPath = System.IO.Directory.GetFiles( MATERIALS_ROOTPATH, "*.mat", SearchOption.AllDirectories );

            ModelProperties props = new ModelProperties();

            int ctr = 0;
            foreach( string mod in modelsPath )
            {
                props = new ModelProperties();

                props.skinMaterials.Clear();
                foreach( string mat in materialsPath )
                {
                    string[] tMat = mat.Split( '/' );
                    string ts = tMat[ tMat.Length - 1 ];

                    ModelMaterialProperty matProp = new ModelMaterialProperty();
                    matProp.asset = ( Material ) AssetDatabase.LoadAssetAtPath( mat, typeof( Material ) );

                    if( ts.Contains( "skin" ) )
                    {
                        string[] tNm = ts.Split( '_' );
                        string name = ( tNm[ tNm.Length - 1 ] ).Replace( ".mat", "" );
                        matProp.displayName = name.ToUpper();
                        props.skinMaterials.Add( matProp );
                    }
                    else if( ts.Contains( "hair_eyes_mouth" ) )
                    {
                        props.headFeatureMaterial = matProp;
                    }
                    else if( ts.Contains( "clothing_top" ) )
                    {
                        props.clothingTopMaterial = matProp;
                    }
                    else if( ts.Contains( "clothing_bottom" ) )
                    {
                        props.clothingBottomMaterial = matProp;
                    }
                }

                if( ( Directory.GetFiles( ( mod + "/" ), "*.fbx", SearchOption.AllDirectories ) ).Length > 0 )
                {
                    string[] tMod = mod.Split( '/' );

                    props.index = ctr;
                    props.rootDir = mod + "/";
                    props.gender = ( tMod[ tMod.Length - 1 ] ).Split( '_' )[ 1 ];

                    // Get reference of the base body (fbx)
                    string[] refModels = Directory.GetFiles( props.rootDir + "base/", "*.fbx" );
                    if( refModels.Length > 0 )
                    {
                        foreach( string refPath in refModels )
                        {
                            if( refPath.Contains( "_char" ) )
                            {
                                props.refCharDir = refPath;
                                props.refCharGo = AssetDatabase.LoadAssetAtPath< GameObject >( props.refCharDir ) as GameObject;
                            }
                            else if( refPath.Contains( "_mouth" ) )
                            {
                                props.refMouthGo = AssetDatabase.LoadAssetAtPath< GameObject >( refPath ) as GameObject;
                            }
                            else if( refPath.Contains( "_eyes" ) )
                            {
                                props.refEyesGo = AssetDatabase.LoadAssetAtPath< GameObject >( refPath ) as GameObject;
                            }
                            else if( refPath.Contains( "_brows" ) )
                            {
                                props.refBrowsGo = AssetDatabase.LoadAssetAtPath< GameObject >( refPath ) as GameObject;
                            }
                        }
                    }

                    // Get reference of the animator controller
                    string[] refAnimControllers = Directory.GetFiles( props.rootDir + "animation/", "*.controller" );
                    if( refAnimControllers.Length > 0 )
                    {
                        foreach( string animPath in refAnimControllers )
                        {
                            if( animPath.Contains( "CharAnimator" ) )
                            {
                                props.refCharAnimController = AssetDatabase.LoadAssetAtPath< AnimatorController >( animPath ) as AnimatorController;
                            }
                            else if( animPath.Contains( "BrowsAnimator" ) )
                            {
                                props.refBrowsAnimController = AssetDatabase.LoadAssetAtPath< AnimatorController >( animPath ) as AnimatorController;
                            }
                            else if( animPath.Contains( "EyesAnimator" ) )
                            {
                                props.refEyesAnimController = AssetDatabase.LoadAssetAtPath< AnimatorController >( animPath ) as AnimatorController;
                            }
                            else if( animPath.Contains( "MouthAnimator" ) )
                            {
                                props.refMouthAnimController = AssetDatabase.LoadAssetAtPath< AnimatorController >( animPath ) as AnimatorController;
                            }
                        }
                    }
                    else
                    {
                        refAnimControllers = Directory.GetFiles( props.rootDir + "animation/", "*.overrideController" );
                        if( refAnimControllers.Length > 0 )
                        {
                            foreach( string animPath in refAnimControllers )
                            {
                                if( animPath.Contains( "CharAnimator" ) )
                                {
                                    props.refCharAnimController = AssetDatabase.LoadAssetAtPath< AnimatorOverrideController >( animPath ) as AnimatorOverrideController;
                                }
                                else if( animPath.Contains( "BrowsAnimator" ) )
                                {
                                    props.refBrowsAnimController = AssetDatabase.LoadAssetAtPath< AnimatorOverrideController >( animPath ) as AnimatorOverrideController;
                                }
                                else if( animPath.Contains( "EyesAnimator" ) )
                                {
                                    props.refEyesAnimController = AssetDatabase.LoadAssetAtPath< AnimatorOverrideController >( animPath ) as AnimatorOverrideController;
                                }
                                else if( animPath.Contains( "MouthAnimator" ) )
                                {
                                    props.refMouthAnimController = AssetDatabase.LoadAssetAtPath< AnimatorOverrideController >( animPath ) as AnimatorOverrideController;
                                }
                            }
                        }
                    }

                    arrModelProperties.Add( props );
                    arrBaseNames.Add( props.gender );
                    arrBaseIndices.Add( props.index );

                    ctr++;
                }
            }
        }

        private void ResetModelProperties()
        {
            selectedBase.selectedSkinIdx = 0;
            selectedBase.selectedBodyIdx = 0;

            for( int i = 0; i < selectedBase.baseBodiesTx.Count; i++ )
            {
                selectedBase.baseBodiesTx[ i ].gameObject.SetActive( i == selectedBase.selectedBodyIdx );
            }

            selectedBase.selectedHairIdx = -1;
            selectedBase.selectedBrowsIdx = -1;
            selectedBase.selectedEyesIdx = 0;
            selectedBase.selectedMouthIdx = 0;
            selectedBase.isBald = false;
            selectedBase.addHair = false;
            selectedBase.addBrows = false;

            selectedBase.selectedTopIdx = -1;
            selectedBase.addTopClothing = false;

            selectedBase.selectedBottomIdx = -1;
            selectedBase.addBottomClothing = false;

            selectedBase.selectedShoesIdx = -1;
            selectedBase.addShoes = false;
        }

        private void CreateModelInstance()
        {
            GameObject instanceGo = ( GameObject ) Instantiate< GameObject >( selectedBase.refCharGo ) as GameObject;
            if( instanceGo != null )
            {
                if( instanceGo.GetComponent< CuztomCharacterAnimatorUtility >() == null )
                {
                    instanceGo.AddComponent< CuztomCharacterAnimatorUtility >();
                }

                if( instanceGo.GetComponent< Animator >() != null )
                {
                    if( selectedBase.refCharAnimController.GetType() == typeof( AnimatorController ) )
                    {
                        instanceGo.GetComponent< Animator >().runtimeAnimatorController = ( AnimatorController ) selectedBase.refCharAnimController;
                    }
                    else if( selectedBase.refCharAnimController.GetType() == typeof( AnimatorOverrideController ) )
                    {
                        instanceGo.GetComponent< Animator >().runtimeAnimatorController = ( AnimatorOverrideController ) selectedBase.refCharAnimController;
                    }
                }

                instanceGo.name = ( !string.IsNullOrEmpty( _newInstanceName ) ) ? _newInstanceName : string.Format( "cuztom_{0}", ( selectedBase.gender ).ToLower() );
                int baseChildCtr = instanceGo.transform.childCount;
                if( baseChildCtr > 0 )
                {
                    for( int a = 0; a < baseChildCtr; a++ )
                    {
                        GameObject baseChildModel = instanceGo.transform.GetChild( a ).gameObject;
                        if( baseChildModel.name.Contains( "mdl_" ) | baseChildModel.name.Contains( "_char" ) )
                        {
                            int modelChild = baseChildModel.transform.childCount;
                            if( modelChild > 0 )
                            {
                                Transform txBodies = baseChildModel.transform.Find( "bodies" );
                                if( !txBodies )
                                {
                                    txBodies = baseChildModel.transform.Find( "body" );
                                }

                                if( txBodies )
                                {
                                    int selectedIdx = selectedBase.selectedBodyIdx;
                                    string selectedName = selectedBase.baseBodiesTx[ selectedIdx ].gameObject.name;

                                    int bodiesCtr = txBodies.childCount;
                                    for( int c = ( bodiesCtr - 1 ); c >= 0; c-- )
                                    {
                                        int subBodiesCtr = txBodies.GetChild( c ).childCount;
                                        if( selectedBase.gender.Contains( "Female" ) )
                                        {
                                            if( subBodiesCtr > 0 )
                                            {
                                                for( int cx = ( subBodiesCtr - 1 ); cx >=0; cx-- )
                                                {
                                                    if( txBodies.GetChild( c ).GetChild( cx ).name != selectedName )
                                                    {
                                                        txBodies.GetChild( c ).GetChild( cx ).gameObject.SetActive( true );
                                                        DestroyImmediate( txBodies.GetChild( c ).GetChild( cx ).gameObject );
                                                    }
                                                }

                                                subBodiesCtr = txBodies.GetChild( c ).childCount;
                                                if( subBodiesCtr ==  0 )
                                                {
                                                    DestroyImmediate( txBodies.GetChild( c ).gameObject );
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if( ( txBodies.GetChild( c ).name != selectedName ) )
                                            {
                                                txBodies.GetChild( c ).gameObject.SetActive( true );
                                                DestroyImmediate( txBodies.GetChild( c ).gameObject );
                                            }
                                        }
                                    }
                                }

                                Transform txHairs = baseChildModel.transform.Find( "hair" );
                                if( txHairs )
                                {
                                    string selectedName = "";
                                    if( selectedBase.addHair )
                                    {
                                        selectedName = txHairs.GetChild( selectedBase.selectedHairIdx ).gameObject.name;
                                    }

                                    for( int c = ( txHairs.childCount - 1 ); c >= 0; c-- )
                                    {
                                        if( ( txHairs.GetChild( c ).name != selectedName ) )
                                        {
                                            txHairs.GetChild( c ).gameObject.SetActive( true );
                                            DestroyImmediate( txHairs.GetChild( c ).gameObject );
                                        }
                                    }
                                }

                                Transform txBrows = baseChildModel.transform.Find( "brows" );
                                if( txBrows )
                                {
                                    DestroyImmediate( txBrows.gameObject );

                                    txBrows = ( Instantiate< GameObject >( selectedBase.refBrowsGo ) ).transform;
                                    txBrows.SetParent( baseChildModel.transform, true );
                                    txBrows.name = "brows";

                                    if( txBrows.GetComponent< Animator >() != null )
                                    {
                                        if( selectedBase.refBrowsAnimController.GetType() == typeof( AnimatorController ) )
                                        {
                                            txBrows.GetComponent< Animator >().runtimeAnimatorController = ( AnimatorController ) selectedBase.refBrowsAnimController;
                                        }
                                        else if( selectedBase.refBrowsAnimController.GetType() == typeof( AnimatorOverrideController ) )
                                        {
                                            txBrows.GetComponent< Animator >().runtimeAnimatorController = ( AnimatorOverrideController ) selectedBase.refBrowsAnimController;
                                        }
                                    }

                                    string selectedName = "";
                                    if( selectedBase.addBrows )
                                    {
                                        selectedName = txBrows.GetChild( selectedBase.selectedBrowsIdx ).gameObject.name;
                                    }

                                    for( int c = ( txBrows.childCount - 1 ); c >= 0; c-- )
                                    {
                                        if( ( txBrows.GetChild( c ).name != selectedName ) )
                                        {
                                            txBrows.GetChild( c ).gameObject.SetActive( true );
                                            DestroyImmediate( txBrows.GetChild( c ).gameObject );
                                        }
                                        else
                                        {
                                            if( instanceGo.transform.Find( "bn_main" ) != null )
                                            {
                                                Transform rootBone = instanceGo.transform.Find( "bn_main/sn_spn1/sn_spn2/bn_chest/bn_neck/bn_head/bn_brows" );
                                                txBrows.GetChild( c ).GetComponent< SkinnedMeshRenderer >().rootBone = rootBone;
                                            }
                                        }
                                    }
                                }

                                Transform txEyes = baseChildModel.transform.Find( "eyes" );
                                if( txEyes )
                                {
                                    DestroyImmediate( txEyes.gameObject );

                                    txEyes = ( Instantiate< GameObject >( selectedBase.refEyesGo ) ).transform;
                                    txEyes.SetParent( baseChildModel.transform, true );
                                    txEyes.name = "eyes";

                                    if( txEyes.GetComponent< Animator >() != null )
                                    {
                                        if( selectedBase.refEyesAnimController.GetType() == typeof( AnimatorController ) )
                                        {
                                            txEyes.GetComponent< Animator >().runtimeAnimatorController = ( AnimatorController ) selectedBase.refEyesAnimController;
                                        }
                                        else if( selectedBase.refEyesAnimController.GetType() == typeof( AnimatorOverrideController ) )
                                        {
                                            txEyes.GetComponent< Animator >().runtimeAnimatorController = ( AnimatorOverrideController ) selectedBase.refEyesAnimController;
                                        }
                                    }

                                    string selectedName = "";
                                    if( selectedBase.selectedEyesIdx >= 0 )
                                    {
                                        selectedName = txEyes.GetChild( selectedBase.selectedEyesIdx ).gameObject.name;
                                    }

                                    for( int c = ( txEyes.childCount - 1 ); c >= 0; c-- )
                                    {
                                        if( ( txEyes.GetChild( c ).name != selectedName ) )
                                        {
                                            txEyes.GetChild( c ).gameObject.SetActive( true );
                                            DestroyImmediate( txEyes.GetChild( c ).gameObject );
                                        }
                                        else
                                        {
                                            if( instanceGo.transform.Find( "bn_main" ) != null )
                                            {
                                                Transform rootBone = instanceGo.transform.Find( "bn_main/sn_spn1/sn_spn2/bn_chest/bn_neck/bn_head/bn_eyes" );
                                                txEyes.GetChild( c ).GetComponent< SkinnedMeshRenderer >().rootBone = rootBone;
                                            }
                                        }
                                    }
                                }

                                Transform txMouths = baseChildModel.transform.Find( "mouth" );
                                if( txMouths )
                                {
                                    DestroyImmediate( txMouths.gameObject );

                                    txMouths = ( Instantiate< GameObject >( selectedBase.refMouthGo ) ).transform;
                                    txMouths.SetParent( baseChildModel.transform, true );
                                    txMouths.name = "mouth";

                                    if( txMouths.GetComponent< Animator >() != null )
                                    {
                                        if( selectedBase.refMouthAnimController.GetType() == typeof( AnimatorController ) )
                                        {
                                            txMouths.GetComponent< Animator >().runtimeAnimatorController = ( AnimatorController ) selectedBase.refMouthAnimController;
                                        }
                                        else if( selectedBase.refMouthAnimController.GetType() == typeof( AnimatorOverrideController ) )
                                        {
                                            txMouths.GetComponent< Animator >().runtimeAnimatorController = ( AnimatorOverrideController ) selectedBase.refMouthAnimController;
                                        }
                                    }

                                    string selectedName = "";
                                    if( selectedBase.selectedMouthIdx > ( -1 ) )
                                    {
                                        selectedName = txMouths.GetChild( selectedBase.selectedMouthIdx ).gameObject.name;
                                    }

                                    for( int c = ( txMouths.childCount - 1 ); c >= 0; c-- )
                                    {
                                        if( ( txMouths.GetChild( c ).name != selectedName ) )
                                        {
                                            txMouths.GetChild( c ).gameObject.SetActive( true );
                                            DestroyImmediate( txMouths.GetChild( c ).gameObject );
                                        }
                                        else
                                        {
                                            if( instanceGo.transform.Find( "bn_main" ) != null )
                                            {
                                                Transform rootBone = instanceGo.transform.Find( "bn_main/sn_spn1/sn_spn2/bn_chest/bn_neck/bn_head/bn_mouth" );
                                                txMouths.GetChild( c ).GetComponent< SkinnedMeshRenderer >().rootBone = rootBone;
                                            }
                                        }
                                    }
                                }

                                Transform txTopClothes = baseChildModel.transform.Find( "top_clothing" );
                                if( txTopClothes )
                                {
                                    string selectedName = "";
                                    if( selectedBase.addTopClothing )
                                    {
                                        selectedName = txTopClothes.GetChild( selectedBase.selectedTopIdx ).gameObject.name;
                                    }

                                    for( int c = ( txTopClothes.childCount - 1 ); c >= 0; c-- )
                                    {
                                        if( ( txTopClothes.GetChild( c ).name != selectedName ) )
                                        {
                                            txTopClothes.GetChild( c ).gameObject.SetActive( true );
                                            DestroyImmediate( txTopClothes.GetChild( c ).gameObject );
                                        }
                                    }
                                }

                                Transform txPants = baseChildModel.transform.Find( "pants" );
                                if( txPants )
                                {
                                    string selectedName = "";
                                    if( selectedBase.addBottomClothing )
                                    {
                                        selectedName = txPants.GetChild( selectedBase.selectedBottomIdx ).gameObject.name;
                                    }

                                    for( int c = ( txPants.childCount - 1 ); c >= 0; c-- )
                                    {
                                        if( ( txPants.GetChild( c ).name != selectedName ) )
                                        {
                                            txPants.GetChild( c ).gameObject.SetActive( true );
                                            DestroyImmediate( txPants.GetChild( c ).gameObject );
                                        }
                                    }
                                }

                                Transform txShoes = baseChildModel.transform.Find( "shoes" );
                                if( txShoes )
                                {
                                    string selectedName = "";
                                    if( selectedBase.addShoes )
                                    {
                                        selectedName = txShoes.GetChild( selectedBase.selectedShoesIdx ).gameObject.name;
                                    }

                                    for( int c = ( txShoes.childCount - 1 ); c >= 0; c-- )
                                    {
                                        if( ( txShoes.GetChild( c ).name != selectedName ) )
                                        {
                                            txShoes.GetChild( c ).gameObject.SetActive( true );
                                            DestroyImmediate( txShoes.GetChild( c ).gameObject );
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                ResetModelProperties();
            }
        }

        private IEnumerator CleanupInstance( Transform pTx, string pTxRemainName )
        {
            if( !string.IsNullOrEmpty( pTxRemainName ) )
            {
                int ctr = pTx.childCount;
                while( ctr >= 0 )
                {
                    pTx.GetChild( ctr ).gameObject.SetActive( true );
                    if( pTx.GetChild( ctr ).name != pTxRemainName )
                    {
                        DestroyImmediate( pTx.GetChild( ctr ).gameObject );
                    }

                    ctr -= 1;
                    yield return null;
                }
            }
        }

        private static EditorWindow _thisWindow = null;
        private Editor _thisEditor = null;
        private string _newInstanceName = "";

        private List< string > arrBaseNames = new List< string >();
        private List< int > arrBaseIndices = new List< int >();
        private List< ModelProperties > arrModelProperties = new List< ModelProperties >();

        private static string MODELS_ROOTPATH = "Assets/MesoGames/Customizable Character/Models/";
        #pragma warning disable
        private static string MODELS_PREFABPATH = "Assets/MesoGames/Customizable Character/Prefabs/";
        #pragma warning restore
        private static string MATERIALS_ROOTPATH = "Assets/MesoGames/Customizable Character/Materials/";

        protected bool isSelectedBaseChanged = false;
        protected int selectedBaseIdx = 0;
        protected ModelProperties selectedBase = null;

        public class ModelMaterialProperty
        {
            public string displayName = "";
            public Material asset = null;
        }

        public class ModelProperties
        {
            public int index = 0;
            public string gender = "";
            public string rootDir = "";

            public string refCharDir = "";
            public GameObject refCharGo = null;
            public object refCharAnimController = null;

            public GameObject refMouthGo = null;
            public object refMouthAnimController = null;

            public GameObject refEyesGo = null;
            public object refEyesAnimController = null;

            public GameObject refBrowsGo = null;
            public object refBrowsAnimController = null;

            #region FEATURES

            public bool addBrows = false;
            public int selectedBodyIdx = 0;
            public List< Transform > baseBodiesTx = new List< Transform >();

            public bool isBald = false;
            public int selectedHairIdx = -1;
            public List< Transform > baseHairsTx = new List< Transform >();

            public bool addHair = false;
            public int selectedBrowsIdx = -1;
            public List< Transform > browsSetsTx = new List< Transform >();

            public int selectedEyesIdx = 0;
            public List< Transform > eyesSetsTx = new List< Transform >();

            public int selectedMouthIdx = 0;
            public List< Transform > mouthSetsTx = new List< Transform >();

            #endregion

            #region CLOTHING

            public bool addTopClothing = false;
            public int selectedTopIdx = 0;
            public List< Transform > baseTopsTx = new List< Transform >();

            public bool addBottomClothing = false;
            public int selectedBottomIdx = 0;
            public List< Transform > baseBottomsTx = new List< Transform >();

            public bool addShoes = false;
            public int selectedShoesIdx = 0;
            public List< Transform > baseShoesTx = new List< Transform >();

            #endregion

            public int selectedSkinIdx = 0;
            public List< ModelMaterialProperty > skinMaterials = new List< ModelMaterialProperty >();
            public ModelMaterialProperty headFeatureMaterial = null;
            public ModelMaterialProperty clothingTopMaterial = null;
            public ModelMaterialProperty clothingBottomMaterial = null;

            public bool isDefaultsSet = false;

            public void ShowDefaults()
            {
                if( isDefaultsSet )
                {
                    return;
                }

                if( refCharGo != null )
                {
                    if( refCharGo.transform.childCount > 0 )
                    {
                        for( int i = 0; i < refCharGo.transform.childCount; i++ )
                        {
                            if( refCharGo.transform.GetChild( i ).name.Contains( "mdl_" ) || refCharGo.transform.GetChild( i ).name.Contains( "_char" ) )
                            {
                                Transform baseTx = refCharGo.transform.GetChild( i );
                                for( int o = 0; o < baseTx.childCount; o++ )
                                {
                                    Transform tx = baseTx.GetChild( o );
                                    for( int x = 0; x < tx.childCount; x++ )
                                    {
                                        switch( tx.name )
                                        {
                                            #region FEATURES
                                            case "bodies":
                                            case "body":
                                                {
                                                    // Reserve position-0 for the default body part
                                                    if( baseBodiesTx.Count == 0 )
                                                    {
                                                        baseBodiesTx.Add( null );
                                                    }

                                                    Transform txBody =  tx.GetChild( x );
                                                    if( txBody.GetComponent< SkinnedMeshRenderer >() != null )
                                                    {
                                                        // Check for head-scalp
                                                        int scalpCtr = txBody.childCount;
                                                        if( scalpCtr > 0 )
                                                        {
                                                            Transform txScalp = null;
                                                            for( int s = 0; s < scalpCtr; s++ )
                                                            {
                                                                txScalp = txBody.GetChild( s );
                                                                if( txScalp.gameObject.GetComponent< SkinnedMeshRenderer >() != null )
                                                                {
                                                                    txScalp.gameObject.GetComponent< SkinnedMeshRenderer >().material = skinMaterials[ 0 ].asset;
                                                                }
                                                                txScalp.gameObject.SetActive( ( s == ( scalpCtr - 1 ) ) );
                                                            }
                                                        }

                                                        txBody.GetComponent< SkinnedMeshRenderer >().material = skinMaterials[ 0 ].asset;
                                                        if( txBody.name.Contains( "_base" ) )
                                                        {
                                                            txBody.gameObject.SetActive( true );
                                                            baseBodiesTx[ 0 ] = txBody;
                                                        }
                                                        else
                                                        {
                                                            txBody.gameObject.SetActive( false );
                                                            baseBodiesTx.Add( txBody );
                                                        }
                                                    }
                                                    else
                                                    {
                                                        int optBodyCtr = txBody.childCount;
                                                        if( optBodyCtr > 0 )
                                                        {
                                                            for( int s = 0; s < optBodyCtr; s++ )
                                                            {
                                                                Transform txBodyOpt = txBody.GetChild( s );
                                                                if( txBodyOpt.gameObject.GetComponent< SkinnedMeshRenderer >() != null )
                                                                {
                                                                    txBodyOpt.gameObject.GetComponent< SkinnedMeshRenderer >().material = skinMaterials[ 0 ].asset;
                                                                }

                                                                txBodyOpt.gameObject.SetActive( false );
                                                                baseBodiesTx.Add( txBodyOpt );
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            case "hair":
                                                {
                                                    baseHairsTx.Add( tx.GetChild( x ) );
                                                    baseHairsTx[ x ].gameObject.SetActive( false );
                                                }
                                                break;
                                            case "eyes":
                                                {
                                                    eyesSetsTx.Add( tx.GetChild( x ) );
                                                    eyesSetsTx[ x ].gameObject.SetActive( ( x == 0 ) );
                                                }
                                                break;
                                            case "brows":
                                                {
                                                    browsSetsTx.Add( tx.GetChild( x ) );
                                                    browsSetsTx[ x ].gameObject.SetActive( ( x == 0 ) );
                                                }
                                                break;
                                            case "mouth":
                                                {
                                                    mouthSetsTx.Add( tx.GetChild( x ) );
                                                    mouthSetsTx[ x ].gameObject.SetActive( ( x == 0 ) );
                                                }
                                                break;
                                            #endregion

                                            #region CLOTHING
                                            case "top_clothing":
                                                {
                                                    if( gender.Contains( "Male" ) )
                                                    {
                                                        //special code for suit sub-type
                                                        if( x == ( tx.childCount - 4 )
                                                            | x == ( tx.childCount - 3 ) )
                                                        {
                                                            tx.GetChild( x ).name = tx.GetChild( x ).name.Replace( "suit", "suitlong" );
                                                        }

                                                        if( x == ( tx.childCount - 2 )
                                                            | x == ( tx.childCount - 1 ) )
                                                        {
                                                            tx.GetChild( x ).name = tx.GetChild( x ).name.Replace( "suit", "suitshort" );
                                                        }
                                                    }
                                                    baseTopsTx.Add( tx.GetChild( x ) );
                                                    baseTopsTx[ x ].gameObject.SetActive( false );
                                                }
                                                break;
                                            case "pants":
                                                {
                                                    baseBottomsTx.Add( tx.GetChild( x ) );
                                                    baseBottomsTx[ x ].gameObject.SetActive( false );
                                                }
                                                break;
                                            case "shoes":
                                                {
                                                    baseShoesTx.Add( tx.GetChild( x ) );
                                                    baseShoesTx[ x ].gameObject.SetActive( false );
                                                }
                                                break;
                                            #endregion
                                        }
                                    }
                                }
                            }

                            isDefaultsSet = true;
                        }
                    }
                }
            }
        }
    }
}
