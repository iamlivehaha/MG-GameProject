﻿using System.Collections;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy
{
    public class Miner : IEnemy
    {
        public float Attackoffset = 5;
        public bool isBlock = false;

        private Quaternion lookRot;

        // Start is called before the first frame update
        public override void Init()
        {
            base.Init();
            m_animator = transform.Find("Visuals/Miner").GetComponent<Animator>();
            m_boxCollider = GetComponent<BoxCollider>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_skeletonComponent = transform.Find("Visuals/Miner").GetComponent<ISkeletonComponent>();
            m_target = GameObject.FindGameObjectWithTag("Player").transform;

            // property setting
            m_animator.SetBool("bpatrol", bPatrol);
        }

        // Update is called once per frame
        void Update()
        {
            //check attack angle
            if (!isBlock)
            {
                LookTarget(m_target.transform);
            }
        }

        private void CheckAttackAngle()
        {
            if (bTargetInAttackRange && bTargetInView)
            {
                float rotz = m_weapon.transform.rotation.eulerAngles.z;
                if (rotz > 40 && rotz < 180)
                {
                    isBlock = true;
                }
                else
                {
                    isBlock = false;
                }
                m_animator.SetBool("isblock", isBlock);

            }
        }
        public override void Attack(GameObject o)
        {
            StartCoroutine(StartAttack(o.transform));
        }

        public override void UnderAttack(GameObject o)
        {
            throw new System.NotImplementedException();
        }
        IEnumerator StartAttack(Transform mTarget)
        {

            while (true)
            {
                if (bTargetInView == false)
                {
                    //m_weapon.transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
                    yield break; ;
                }
                if (bTargetInAttackRange)//attack and rest for a interval
                {
                    CheckAttackAngle();
                    m_animator.SetTrigger("attack");
                    yield return new WaitForSeconds(m_attackInterval);
                    isBlock = false;
                    m_animator.SetBool("isblock", isBlock);
                }
                else // no target so do nothing
                {
                    yield return null;
                }

            }
        }
        private void LookTarget(Transform mTarget)
        {
            Vector3 attackDir = (mTarget.position - transform.position).normalized;
            float angle = Vector3.Angle(new Vector3(attackDir.x > 0 ? 1 : -1, 0, 0), attackDir);
            lookRot = Quaternion.AngleAxis(-angle + Attackoffset, Vector3.forward);
            m_weapon.transform.rotation = Quaternion.Slerp(m_weapon.transform.rotation, lookRot, Time.deltaTime);
            //m_weapon.transform.rotation = rot;
        }
    }
}
