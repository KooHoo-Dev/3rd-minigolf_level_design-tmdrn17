using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MiniGolf.KSG
{
    public class StageGenerator : MonoBehaviour
    {
        [Header("타일 및 기믹 프리팹")] [SerializeField]
        private float tileSize = 2f;

        [SerializeField] private GameObject normalTile;
        [SerializeField] private GameObject sandTile;
        [SerializeField] private GameObject boostTile;
        [SerializeField] private GameObject jumpTile;
        [SerializeField] private GameObject laserObstacle;
        [SerializeField] private GameObject holePrefab;
        [SerializeField] private GameObject ballPrefab;

        [Header("생성위치 루트")] [SerializeField] private Transform mapParent;

        private void ResetContainer()
        {
            if (mapParent == null) mapParent = transform;

            // 기존 생성된 맵 타일 싹 제거 (에디터 전용)
            for (int i = mapParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(mapParent.GetChild(i).gameObject);
            }
        }

        [ContextMenu("0. 맵 초기화 (Clear)")]
        public void ClearMap()
        {
            ResetContainer();
        }

        [ContextMenu("1. Generate Stage 1 (기본 & 모래 & 다리)")]
        public void GenerateStage1()
        {
            ResetContainer();

            // Start Point (0,0)
            Spawn(ballPrefab, new Vector3(0, 0.5f, 0));
            Spawn(normalTile, new Vector3(0, 0, 0));
            Spawn(normalTile, new Vector3(0, 0, 1));

            // 모래 구간 (0, 2~3)
            Spawn(sandTile, new Vector3(0, 0, 2));
            Spawn(sandTile, new Vector3(0, 0, 3));

            // 다리 구간 (0, 4~5)
            Spawn(normalTile, new Vector3(0, 0, 4));
            Spawn(normalTile, new Vector3(0, 0, 5));

            // 홀인 지점 (0, 6)
            Spawn(holePrefab, new Vector3(0, 0, 6));
        }

        [ContextMenu("2. Generate Stage 2 (부스트 & 점프)")]
        public void GenerateStage2()
        {
            ResetContainer();

            // Start
            Spawn(ballPrefab, new Vector3(0, 0.5f, 0));
            Spawn(normalTile, new Vector3(0, 0, 0));

            // 가속 + 점프대
            Spawn(boostTile, new Vector3(0, 0, 1));
            Spawn(jumpTile, new Vector3(0, 0, 2));

            // (3~4번 위치는 낙사용 빈 공간 - 절벽)

            // 착지 지점 (모래로 감속)
            Spawn(sandTile, new Vector3(0, 0, 5));
            Spawn(normalTile, new Vector3(0, 0, 6));

            // 홀인 지점
            Spawn(holePrefab, new Vector3(0, 0, 7));
        }

        [ContextMenu("3. Generate Stage 3 (레이저 & 폭탄 타이밍)")]
        public void GenerateStage3()
        {
            ResetContainer();

            // Start
            Spawn(ballPrefab, new Vector3(0, 0.5f, 0));
            Spawn(normalTile, new Vector3(0, 0, 0));

            // L자 곡선 코스 + 레이저
            Spawn(normalTile, new Vector3(0, 0, 1));
            Spawn(laserObstacle, new Vector3(0, 0.5f, 1)); // 레이저 배치

            Spawn(normalTile, new Vector3(0, 0, 2));

            // 오른쪽 꺾임 코스 (1,3 ~ 3,3)
            Spawn(normalTile, new Vector3(1, 0, 3));
            Spawn(normalTile, new Vector3(2, 0, 3));
            Spawn(laserObstacle, new Vector3(2, 0.5f, 3));

            // 홀인 지점
            Spawn(holePrefab, new Vector3(3, 0, 3));
        }

        [ContextMenu("4. Generate Stage 4 (도망가는 골대 & 우회 코스)")]
        public void GenerateStage4()
        {
            ResetContainer();

            // Start
            Spawn(ballPrefab, new Vector3(0, 0.5f, 0));
            Spawn(normalTile, new Vector3(0, 0, 0));
            Spawn(normalTile, new Vector3(0, 0, 1));

            // 정면 골대 (앞으로 접근하면 도망치는 골대)
            Spawn(holePrefab, new Vector3(0, 0, 3));

            // 우회용 부스트 패드 코스 (오른쪽 우회)
            Spawn(normalTile, new Vector3(1, 0, 1));
            Spawn(boostTile, new Vector3(1, 0, 2));

            // 반사 폭탄으로 골대 뒤로 공을 튕겨 넣는 각도
            Spawn(normalTile, new Vector3(1, 0, 4));

            Spawn(normalTile, new Vector3(0, 0, 4)); // 골대 뒤쪽 진입 타일
        }

        [ContextMenu("5. Generate Stage 5 (클라이맥스 종합 코스)")]
        public void GenerateStage5()
        {
            ResetContainer();

            // Start
            Spawn(ballPrefab, new Vector3(0, 0.5f, 0));
            Spawn(normalTile, new Vector3(0, 0, 0));

            // 1구간: 가속 + 레이저
            Spawn(boostTile, new Vector3(0, 0, 1));
            Spawn(normalTile, new Vector3(0, 0, 2));
            Spawn(laserObstacle, new Vector3(0, 0.5f, 2));

            // 2구간: 점프대 + 절벽 통과
            Spawn(jumpTile, new Vector3(0, 0, 3));
            // (4, 5번은 절벽)

            // 3구간: 착지 모래 타일 + 도망가는 골대
            Spawn(sandTile, new Vector3(0, 0, 6));
            Spawn(normalTile, new Vector3(0, 0, 7));
            Spawn(holePrefab, new Vector3(0, 0, 8));
        }

        private GameObject Spawn(GameObject prefab, Vector3 gridPos, Quaternion rotation = default)
        {
            if (prefab == null) return null;

            Vector3 worldPos = new Vector3(gridPos.x * tileSize, gridPos.y, gridPos.z * tileSize);

#if UNITY_EDITOR
            // Undo 지원 및 프리팹 연결 유지를 위해 PrefabUtility 사용
            GameObject obj = PrefabUtility.InstantiatePrefab(prefab, mapParent) as GameObject;
            if (obj == null) obj = Instantiate(prefab, mapParent);
#else
        GameObject obj = Instantiate(prefab, mapParent);
#endif

            obj.transform.position = transform.position + worldPos;
            obj.transform.rotation = rotation;
            return obj;
        }
    }
}