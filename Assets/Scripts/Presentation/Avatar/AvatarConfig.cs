using UnityEngine;

public enum AvatarAge { Baby, Adult }

[System.Serializable] public class AvatarLayerSet
{
    [Header("Features")]
    public Sprite features;                  // sẹo, tàn nhang, má hồng, nếp nhăn

    [Header("Head (đầu & mặt)")]
    public Sprite eyes;
    public Sprite eyebrows;
    public Sprite mouth;

    [Header("Hair (tóc)")]
    public Sprite hairBack;                  // tóc lớp sau (nằm sau head)
    public Sprite hairFront;                 // tóc lớp trước (phủ lên head)

    [Header("Facial Hair (râu) - người lớn")]
    public Sprite beard;                     // bé sơ sinh để trống

    [Header("Accessories (tuỳ chọn)")]
    public Sprite accessory;                 // nón, kính, vv. (để trống nếu chưa dùng)
}

[CreateAssetMenu(menuName="LifeSim/Avatar Config", fileName="AvatarConfig")]
public class AvatarConfig : ScriptableObject
{
    [Header("Cấu hình cho từng lứa tuổi")]
    public AvatarLayerSet baby;    // cho Age = Baby
    public AvatarLayerSet adult;   // cho Age = Adult

    [Header("Thứ tự vẽ (z-order)")]
    // 0 = vẽ trước; số lớn vẽ sau (lớp trên)
    public int z_hairBack   = 1;
    public int z_features   = 2;
    public int z_eyes       = 3;
    public int z_eyebrows   = 4;
    public int z_beard      = 5;
    public int z_mouth      = 6;
    public int z_hairFront  = 7;
    public int z_accessory  = 8;

    public AvatarLayerSet GetSet(AvatarAge age) => age == AvatarAge.Baby ? baby : adult;
}