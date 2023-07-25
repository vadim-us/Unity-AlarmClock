using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float _playerMoveSpeed = 5f;
    float _xRotation;
    float _rotSens = 3f;
    Camera _camera;

    [SerializeField]
    Transform _enemy;
    [SerializeField]
    Transform _gun;
    Transform _targetPoint;
    [SerializeField]
    GameObject _bullet;
    float _bulletSpeed = 10f;

    Transform _bulletCreator;

    Vector3 _enemyCurrentPos;
    Vector3 _enemyPredPos;
    Vector3 _enemyMoveDir;
    float _enemySpeed;
    float _enemyMoveDis;

    void Start()
    {
        _camera = transform.GetComponentInChildren<Camera>();
        _bulletCreator = _gun.GetChild(0);
        _targetPoint = _enemy.GetChild(0);
        _enemyPredPos = _enemy.position;
    }

    void Update()
    {
        GunFire();

        MouseRotation();
        GamepadRotation();
        PlayerMove();

        RaycastHit hit = DrawLineTrace(_camera.transform.position, _camera.transform.forward, Color.yellow);

        DrawLineTrace(_bulletCreator.position, _bulletCreator.forward, Color.red, hit);
        DrawLineTrace(_bulletCreator.position, _bulletCreator.forward, Color.green);
    }

    private void LateUpdate()
    {
        TargetPos();
    }

    private void FixedUpdate()
    {

    }

    private void TargetPos()
    {
        if (_enemy != null && _enemy.gameObject.activeInHierarchy)
            Debug.LogWarning("!");
        _enemyCurrentPos = _enemy.position;
        if (_enemyCurrentPos != _enemyPredPos)
        {
            Vector3 newPos = _enemyCurrentPos - _enemyPredPos;
            _enemyMoveDis = newPos.magnitude;
            _enemyMoveDir = newPos / _enemyMoveDis;
            _enemySpeed = _enemyMoveDis / Time.deltaTime;

            _enemyPredPos = _enemyCurrentPos;

            //==================================

            float timeToEnemy = (_enemyCurrentPos - _bulletCreator.position).magnitude / _bulletSpeed;
            Vector3 targetPointPos = _enemyMoveDir * _enemySpeed * timeToEnemy;
            _targetPoint.localPosition = targetPointPos;

            Debug.DrawRay(_enemyCurrentPos, _enemyMoveDir * 5f, Color.green);
        }
    }

    ///<summary>Огонь из оружия</summary>
    private void GunFire()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            GameObject bullet = Instantiate(_bullet, _bulletCreator.position, _bulletCreator.rotation);

            Physics.IgnoreCollision(_gun.GetComponent<Collider>(), bullet.GetComponentInChildren<Collider>());
            bullet.GetComponent<Rigidbody>().velocity = _bulletCreator.forward * _bulletSpeed;
        }
    }

    private RaycastHit DrawLineTrace(Vector3 pos, Vector3 dir, Color color, RaycastHit hit = new RaycastHit())
    {
        Ray ray = new Ray(pos, dir);
        if (hit.distance > 0)
        {
            _gun.LookAt(hit.point);
            Vector3 gg = _gun.localEulerAngles;
            gg.z = 0;
            _gun.localEulerAngles = gg;
            //cunRotation = cun.rotation;
            Debug.DrawRay(ray.origin, ray.direction * 30f, color);
        }
        else
        {
            Physics.Raycast(ray, out hit);
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, color);
        }

        return hit;
    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 5;
        Gizmos.DrawRay(transform.position, direction);
    }*/

    void MouseRotation()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;

            _xRotation -= Input.GetAxis("Mouse Y") * _rotSens;
            _xRotation = Mathf.Clamp(_xRotation, -60f, 60f);
            _camera.transform.localEulerAngles = new Vector3(_xRotation, 0f, 0f);

            transform.Rotate(0, Input.GetAxis("Mouse X") * _rotSens, 0);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void GamepadRotation()
    {
        //if (Input.GetAxis("Gamepad Y")!=0 || Input.GetAxis("Gamepad X")!=0)
        //{
            //Cursor.lockState = CursorLockMode.Locked;

            _xRotation -= Input.GetAxis("Gamepad Y");// * _rotSens;
            _xRotation = Mathf.Clamp(_xRotation, -60f, 60f);
            _camera.transform.localEulerAngles = new Vector3(_xRotation, 0f, 0f);

            transform.Rotate(0, Input.GetAxis("Gamepad X"), 0);// * _rotSens, 0
            //Cursor.lockState = CursorLockMode.None;
        //}
    }

    void PlayerMove()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(horizontalInput, 0, verticalInput) * _playerMoveSpeed * Time.deltaTime);
    }
}