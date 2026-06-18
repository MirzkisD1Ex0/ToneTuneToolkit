/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
using UnityEngine;

public class Mesh2PointCloudHandler : MonoBehaviour
{
  private ParticleSystem ps;
  private Mesh mesh;

  private Vector3[] originalVertices; // �����ʼ����λ��

  [SerializeField] private float particleSize = 0.01f;
  [SerializeField] private bool isRandom = false;
  [SerializeField] private int particlesPerFrame = 80;

  // ==================================================

  private void Start() => Init();
  private void Update() => DEBUG_ShortcutKey();
  private void OnDestroy() => UnInit();

  // ==================================================

  private void Init()
  {
    mr = GetComponent<MeshRenderer>();
    mesh = GetComponent<MeshFilter>().sharedMesh;
    ps = GetComponent<ParticleSystem>();
    ps.Stop();

    // Application.targetFrameRate = 165;

    originalVertices = mesh.vertices;
  }

  private void UnInit()
  {
    ClearPointCloud();
  }

  public void Reset()
  {
    ps.Stop();
    ClearPointCloud();
    // GetComponent<ParticleSystemRenderer>().material = new Material(Shader.Find("Universal Render Pipeline/Particles/Lit"));
    GetComponent<ParticleSystemRenderer>().material = new Material(Shader.Find("Universal Render Pipeline/Particles/Lit")); // ���Ӳ�����urp�ģ��������ɫ�����ﻻshader
  }

  // ==================================================
  #region ����

  public void Play()
  {
    ClearPointCloud();
    GeneratePointCloud();
  }

  public void Stop()
  {
    StopCoroutine(GeneratePointCloudAction());
    ClearPointCloud();
  }

  #endregion
  // ==================================================

  private void ClearPointCloud()
  {
    ps.Clear();
    // GetComponentInChildren<ParticleSystem>().enableEmission = false;
  }

  private void GeneratePointCloud() => StartCoroutine(GeneratePointCloudAction());
  private IEnumerator GeneratePointCloudAction()
  {
    int[] shuffledIndices = GetShuffledIndices(mesh.vertexCount);

    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[mesh.vertexCount];
    for (int i = 0; i < mesh.vertexCount; i += particlesPerFrame)
    {
      for (int j = 0; j < particlesPerFrame && i + j < mesh.vertexCount; j++)
      {
        int vertexIndex = isRandom ? shuffledIndices[i + j] : i + j;
        Vector3 vertex = originalVertices[vertexIndex];

        Color color = MeshVerticesColor(mesh, mr.material, vertexIndex);

        particles[i + j].position = vertex;
        particles[i + j].startColor = color;
        particles[i + j].startSize = particleSize; // ������Ҫ�������Ӵ�С
      }

      ps.SetParticles(particles, i + particlesPerFrame); // ����������

      yield return new WaitForEndOfFrame(); // �ȴ���һ֡
    }
  }

  // ==================================================

  // ��ȡ�����������������
  private int[] GetShuffledIndices(int count)
  {
    int[] indices = new int[count];
    for (int i = 0; i < count; i++)
    {
      indices[i] = i;
    }

    // ʹ�� Fisher-Yates �㷨�����������
    System.Random random = new System.Random();
    for (int i = count - 1; i > 0; i--)
    {
      int j = random.Next(0, i + 1);
      int temp = indices[i];
      indices[i] = indices[j];
      indices[j] = temp;
    }

    return indices;
  }

  private Color MeshVerticesColor(Mesh mesh, Material material, int vertexIndex)
  {
    Vector3[] vertices = mesh.vertices;
    Vector2[] uv = mesh.uv;

    if (material.mainTexture != null && vertexIndex >= 0 && vertexIndex < vertices.Length)
    {
      Vector3 vertexPosition = transform.TransformPoint(vertices[vertexIndex]);
      Vector2 uvCoordinate = uv[vertexIndex];

      uvCoordinate.x = Mathf.Repeat(uvCoordinate.x, 1f);
      uvCoordinate.y = Mathf.Repeat(uvCoordinate.y, 1f);

      Texture mainTexture = material.mainTexture;
      Color textureColor = ((Texture2D)mainTexture).GetPixelBilinear(uvCoordinate.x, uvCoordinate.y);

      return textureColor;
    }
    else
    {
      return Color.white;
    }
  }

  // ==================================================
  #region ��Ⱦ������

  private MeshRenderer mr;
  public void SwitchMeshRenderer(bool value) => mr.enabled = value;

  #endregion
  // ==================================================

  private void DEBUG_ShortcutKey()
  {
    if (Input.GetKeyDown(KeyCode.Q)) { Play(); }
    if (Input.GetKeyDown(KeyCode.W)) { Stop(); }
    if (Input.GetKeyDown(KeyCode.A)) { SwitchMeshRenderer(true); }
    if (Input.GetKeyDown(KeyCode.S)) { SwitchMeshRenderer(false); }
  }
}
