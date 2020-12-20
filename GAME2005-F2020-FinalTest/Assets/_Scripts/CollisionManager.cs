using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionManager : MonoBehaviour
{
    public CubeBehaviour[] cubes;
    //public BulletBehaviour[] sphere;
    public BulletBehaviour[] bullets;

    private static Vector3[] faces;

    // Start is called before the first frame update
    void Start()
    {
        cubes = FindObjectsOfType<CubeBehaviour>();

        faces = new Vector3[]
        {
            Vector3.left, Vector3.right,
            Vector3.down, Vector3.up,
            Vector3.back , Vector3.forward
        };
    }

    // Update is called once per frame
    void Update()
    {
        bullets = FindObjectsOfType<BulletBehaviour>();
        //bullets = FindObjectsOfType<CubeBehaviour>();

        // check each AABB with every other AABB in the scene
        for (int i = 0; i < cubes.Length; i++)
        {
            for (int j = 0; j < cubes.Length; j++)
            {
                if (i != j)
                {
                    CheckAABBs(cubes[i], cubes[j]);
                }
            }
        }

        // Check each sphere against each AABB in the scene
        //foreach (var sphere in sphere)
        foreach (var bullet in bullets)
        {
            foreach (var cube in cubes)
            {
                if (cube.name != "Player")
                {
                    CheckBulletAABB(bullet, cube);
                }       
            }
        }
    }

    public static void CheckBulletAABB(BulletBehaviour s, CubeBehaviour b)
    {
        

        Contact contactB = new Contact(b);

        if ((s.min.x <= b.max.x && s.max.x >= b.min.x) &&
            (s.min.y <= b.max.y && s.max.y >= b.min.y) &&
            (s.min.z <= b.max.z && s.max.z >= b.min.z))
        {
            // determine the distances between the contact extents
            float[] distances = {
                (b.max.x - s.min.x),
                (s.max.x - b.min.x),
                (b.max.y - s.min.y),
                (s.max.y - b.min.y),
                (b.max.z - s.min.z),
                (s.max.z - b.min.z)
            };

            float penetration = float.MaxValue;
            Vector3 face = Vector3.zero;

            // check each face to see if it is the one that connected
            for (int i = 0; i < 6; i++)
            {
                if (distances[i] < penetration)
                {
                    // determine the penetration distance
                    penetration = distances[i];
                    face = faces[i];
                }
            }

            // set the contact properties
            contactB.face = face;
            contactB.penetration = penetration;

            // check if contact does not exist
            if (!s.contacts.Contains(contactB))
            {
                // remove any contact that matches the name but not other parameters
                for (int i = s.contacts.Count - 1; i > -1; i--)
                {
                    if (s.contacts[i].cube.name.Equals(contactB.cube.name))
                    {
                        s.contacts.RemoveAt(i);
                    }
                }

                if (contactB.face == Vector3.down)
                {
                    //s.gameObject.GetComponent<RigidBody3D>().Stop();
                    //s.isGrounded = true;
                }
                s.penetration = penetration;
                s.collisionNormal = face;
                // add the new contact
                s.contacts.Add(contactB);
                s.isColliding = true;

                Reflect(s);

            }
        }
        else
        {

            if (s.contacts.Exists(x => x.cube.gameObject.name == b.gameObject.name))
            {
                s.contacts.Remove(s.contacts.Find(x => x.cube.gameObject.name.Equals(b.gameObject.name)));
                s.isColliding = false;

            }
        }
        //// get box closest point to sphere center by clamping
        //var x = Mathf.Max(b.min.x, Mathf.Min(s.transform.position.x, b.max.x));
        //var y = Mathf.Max(b.min.y, Mathf.Min(s.transform.position.y, b.max.y));
        //var z = Mathf.Max(b.min.z, Mathf.Min(s.transform.position.z, b.max.z));

        //float penetration = float.MaxValue;
        //Vector3 face = Vector3.zero;



        //var distance = Math.Sqrt((x - s.transform.position.x) * (x - s.transform.position.x) +
        //                         (y - s.transform.position.y) * (y - s.transform.position.y) +
        //                         (z - s.transform.position.z) * (z - s.transform.position.z));

        //if ((distance < s.radius) && (!s.isColliding))
        //{
        //    // determine the distances between the contact extents
        //    float[] distances = {
        //        (b.max.x - s.transform.position.x),
        //        (s.transform.position.x - b.min.x),
        //        (b.max.y - s.transform.position.y),
        //        (s.transform.position.y - b.min.y),
        //        (b.max.z - s.transform.position.z),
        //        (s.transform.position.z - b.min.z)
        //    };

        //    float penetration = float.MaxValue;
        //    Vector3 face = Vector3.zero;

        //    // check each face to see if it is the one that connected
        //    for (int i = 0; i < 6; i++)
        //    {
        //        if (distances[i] < penetration)
        //        {
        //            // determine the penetration distance
        //            penetration = distances[i];
        //            face = faces[i];
        //        }
        //    }

        //    s.penetration = penetration;
        //    s.collisionNormal = face;
        //    //s.isColliding = true;


        //    Reflect(s);
        //}

    }

    // This helper function reflects the bullet when it hits an AABB face
    private static void Reflect(BulletBehaviour s)
    {
        if ((s.collisionNormal == Vector3.forward) || (s.collisionNormal == Vector3.back))
        {
            s.direction = new Vector3(s.direction.x, s.direction.y, -s.direction.z);
        }
        else if ((s.collisionNormal == Vector3.right) || (s.collisionNormal == Vector3.left))
        {
            s.direction = new Vector3(-s.direction.x, s.direction.y, s.direction.z);
        }
        else if ((s.collisionNormal == Vector3.up) || (s.collisionNormal == Vector3.down))
        {
            s.direction = new Vector3(s.direction.x, -s.direction.y, s.direction.z);
        }
    }


    public static void CheckAABBs(CubeBehaviour a, CubeBehaviour b)
    {
        Contact contactB = new Contact(b);

        if ((a.min.x <= b.max.x && a.max.x >= b.min.x) &&
            (a.min.y <= b.max.y && a.max.y >= b.min.y) &&
            (a.min.z <= b.max.z && a.max.z >= b.min.z))
        {
            // determine the distances between the contact extents
            float[] distances = {
                (b.max.x - a.min.x),
                (a.max.x - b.min.x),
                (b.max.y - a.min.y),
                (a.max.y - b.min.y),
                (b.max.z - a.min.z),
                (a.max.z - b.min.z)
            };

            float penetration = float.MaxValue;
            Vector3 face = Vector3.zero;

            // check each face to see if it is the one that connected
            for (int i = 0; i < 6; i++)
            {
                if (distances[i] < penetration)
                {
                    // determine the penetration distance
                    penetration = distances[i];
                    face = faces[i];
                    switch (i)
                    {
                        case 1:
                            //a.canMoveLeft = false;
                            //b.canMoveLeft = false;
                            break;
                        case 3:
                            a.canMoveRight = false;


                            //b.canMoveRight = false;
                            break;
                        case 4:
                            a.canMoveBack = false;
                           // b.canMoveBack = false;
                            break;
                        case 5:
                            a.canMoveForward = false;
                            //b.canMoveForward = false;
                            break;
                        default:
                            break;
                    }
                }
                else if (distances[i] > penetration)
                {
                    switch (i)
                    {
                        case 1:
                            //a.canMoveLeft = true;
                            
                           // b.canMoveLeft = true;
                            break;
                        case 3:
                            a.canMoveRight = true;
                           // b.canMoveRight = true;
                            break;
                        case 4:
                            a.canMoveBack = true;
                           // b.canMoveBack = true;
                            break;
                        case 5:
                            a.canMoveForward = true;
                            //b.canMoveForward = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            
            // set the contact properties
            contactB.face = face;
            contactB.penetration = penetration;


            // check if contact does not exist
            if (!a.contacts.Contains(contactB))
            {
                // remove any contact that matches the name but not other parameters
                for (int i = a.contacts.Count - 1; i > -1; i--)
                {
                    if (a.contacts[i].cube.name.Equals(contactB.cube.name))
                    {
                        a.contacts.RemoveAt(i);
                    }
                }

                if (contactB.face == Vector3.down)
                {
                    a.gameObject.GetComponent<RigidBody3D>().Stop();
                    a.isGrounded = true;
                }
                
                // add the new contact
                a.contacts.Add(contactB);
                a.isColliding = true;
                
            }
        }
        else
        {
            if (a.contacts.Exists(x => x.cube.gameObject.name == b.gameObject.name))
            {
                a.contacts.Remove(a.contacts.Find(x => x.cube.gameObject.name.Equals(b.gameObject.name)));
                a.isColliding = false;

                if (a.gameObject.GetComponent<RigidBody3D>().bodyType == BodyType.DYNAMIC)
                {
                    a.gameObject.GetComponent<RigidBody3D>().isFalling = true;
                    a.isGrounded = false;
                }
            }
        }
    }
}
