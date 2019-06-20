using UnityEngine;

public class BaseTile : MonoBehaviour {
	private int gridPosX, gridPosZ = -1;
	
	public void SetPosition(int x, int z, float tileSize) {
		gridPosX = x;
		gridPosZ = z;
		
		transform.position = new Vector3(x * tileSize, 0, z * tileSize);
	}

	public void SetColor(bool even) {
		Renderer rend = GetComponent<Renderer>();
		MaterialPropertyBlock matPropBlock = new MaterialPropertyBlock();

		Color c = even ? Color.red : Color.blue;
		
		rend.GetPropertyBlock(matPropBlock);
		matPropBlock.SetColor("_BaseColor", c);
		rend.SetPropertyBlock(matPropBlock);
	}
} 