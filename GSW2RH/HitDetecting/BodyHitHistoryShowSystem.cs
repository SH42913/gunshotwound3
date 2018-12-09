using System.Drawing;
using GunshotWound2.GswWorld;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.HitDetecting
{
    [EcsInject]
    public class BodyHitHistoryShowSystem : IEcsRunSystem
    {
        private EcsFilter<GswPedComponent, BodyHitHistoryComponent> _peds;
        
        public void Run()
        {
            foreach (int i in _peds)
            {
                Ped ped = _peds.Components1[i].ThisPed;
                PedBoneId?[] history = _peds.Components2[i].LastDamagedBones;

                for (int historyIndex = 0; historyIndex < history.Length; historyIndex++)
                {
                    PedBoneId? boneId = history[historyIndex];
                    if (boneId == null || !ped.IsBoneValid(boneId.Value)) continue;

                    Color color;
                    float radius;
                    switch (historyIndex)
                    {
                        case 0:
                            color = Color.DarkOrange;
                            radius = 0.15f;
                            break;
                        case 1:
                            color = Color.Yellow;
                            radius = 0.14f;
                            break;
                        case 2:
                            color = Color.ForestGreen;
                            radius = 0.13f;
                            break;
                        default:
                            continue;
                    }

                    PedBoneId bone = boneId.Value;
                    Vector3 position = ped.GetBonePosition(bone);
                    Debug.DrawSphereDebug(position, radius, color);
                }
            }
        }
    }
}