using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    [SerializeField] private ParticleSystem _particleSystem;

    private bool _selected = false;
    private Board _board;
    private bool _killed = false;
    private bool _animate = false;
    private float _animateX = 0;
    private float _animateY = 0;

    public float AnimationSpeed = 0.1f;

    private TileDefinition _tileDefinition;


    public void SetupTile(TileDefinition tileDefinition)
    {
        _tileDefinition = tileDefinition;
        SpriteRenderer.sprite = tileDefinition.TileSprite;
        _board = transform.parent.GetComponent<Board>();

        _particleSystem.startColor = _tileDefinition.EffectColor;
    }

    private void Update()
    {
        if (_animate)
        {
            if (transform.position.y > _animateY)
            {
                transform.position -= new Vector3(0, AnimationSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = new Vector3(_animateX, _animateY);
                _animate = false;
            }
        }
    }

    private void OnMouseEnter()
    {
        if (!_animate && Input.GetMouseButton(0))
            _board.AddToChain(this);
    }

    public void OnMouseDown()
    {
        if (!_animate)
            _board.AddToChain(this);
    }


    public bool IsSelected()
    {
        return _selected;
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            EmitParticles(true);
            SpriteRenderer.sprite = _tileDefinition.TileSpriteSelected;
        }
        else
        {
            EmitParticles(false);
            SpriteRenderer.sprite = _tileDefinition.TileSprite;
        }


        _selected = selected;
    }

    public int GetTileType()
    {
        return _tileDefinition.GetTypeId();
    }

    public void Kill()
    {
        _killed = true;
    }

    public bool IsKilled()
    {
        return _killed;
    }

    public void AnimateTo(Vector2 vecPos)
    {
        _animate = true;
        _animateX = vecPos.x;
        _animateY = vecPos.y;
    }

    public void PlaySound(float pitch)
    {
        transform.GetComponent<AudioSource>().pitch = pitch;
        transform.GetComponent<AudioSource>().Play();
    }


    private void EmitParticles(bool play)
    {
        if (play)
            _particleSystem.Play();
        else
            _particleSystem.Stop();
    }

    public void Destroy()
    {
        _particleSystem.Stop();
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        Destroy(gameObject, _tileDefinition.TileSelectAudio.length);
    }
}