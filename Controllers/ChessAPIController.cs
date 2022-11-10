using ChessAPI.Data;
using ChessAPI.Models;
using Microsoft.AspNetCore.Mvc;
using ChessAPI.Dtos;
using System.Net.Http.Headers;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ChessAPI.Controllers
{
    [Route("api")]
    [ApiController]

    public class ChessAPIControllers : Controller
    {
        private readonly IChessAPIRepo _repository;
        public ChessAPIControllers(IChessAPIRepo repo)
        {
            _repository = repo;
        }
        [HttpGet("GetVersion")]
        public ActionResult<string> GetVersion()
        {
            return Ok("1.0.0");
        }

        [HttpGet("GetLogo")]
        public ActionResult GetLogo()
        {
            string path = Directory.GetCurrentDirectory();
            string imageDir = Path.Combine(path, "Logos");
            string filename = Path.Combine(imageDir, "Logo.png");
            string respHeader = "";
            string fileName = "";
            if (System.IO.File.Exists(filename))
            {
                respHeader = "image/png";
                fileName = filename;
            }
            else
            {
                return NotFound();
            }
            return PhysicalFile(fileName, respHeader);
        }
        [HttpGet("GetFavIcon")]
        public ActionResult GetFavIcon()
        {
            string path = Directory.GetCurrentDirectory();
            string imageDir = Path.Combine(path, "Logos");
            string filename = Path.Combine(imageDir, "Logo-192x192.png");
            string respHeader = "";
            string fileName = "";
            if (System.IO.File.Exists(filename))
            {
                respHeader = "image/png";
                fileName = filename;
            }
            else
            {
                return NotFound();
            }
            return PhysicalFile(fileName, respHeader);
        }
        [HttpGet("ItemPhoto/{id}")]
        public ActionResult ItemPhoto(Int64 id)
        {
            string path = Directory.GetCurrentDirectory();
            string imageDir = Path.Combine(path, "ItemsImages");
            string PngFilename = Path.Combine(imageDir, id + ".png");
            string JpgFilename = Path.Combine(imageDir, id + ".jpg");
            string GifFilename = Path.Combine(imageDir, id + ".gif");
            string respHeader = "";
            string fileName = "";
            if (System.IO.File.Exists(JpgFilename))
            {
                respHeader = "image/jpeg";
                fileName = JpgFilename;
            }
            else if (System.IO.File.Exists(PngFilename))
            {
                respHeader = "image/png";
                fileName = PngFilename;

            }
            else if (System.IO.File.Exists(GifFilename))
            {
                respHeader = "image/gif";
                fileName = GifFilename;
            }
            else
            {
                respHeader = "image/png";
                fileName = Path.Combine(imageDir, "default.png");
            }
            return PhysicalFile(fileName, respHeader);
        }
        [HttpGet("AllItems")]
        public ActionResult<IEnumerable<Product>> AllItems()
        {
            IEnumerable<Product> items = _repository.AllItems();
            IEnumerable<Product> c = items.Select(e => new Product
            { ID = e.ID, Name = e.Name, Price = e.Price, Description = e.Description });
            return Ok(c);

        }
        [HttpGet("GetItems/{name}")]
        public ActionResult<IEnumerable<Product>> GetItems(string name)
        {
            IEnumerable<Product> items = _repository.GetItems();
            IEnumerable<Product> c = items.Where(e => (e.Name.ToLower()).Contains(name.ToLower()));

            return Ok(c);
        }
        [HttpPost("WriteComment")]
        public ActionResult<string> WriteComment(CommentDto comment)
        {
            
            Comment c = new Comment { UserComment = comment.UserComment, Name = comment.Name};
            Comment addedComement = _repository.WriteComment(c);
            CommentDto co = new CommentDto { Name = addedComement.Name, UserComment = addedComement.UserComment};
            CreatedAtAction(nameof(GetComments), new { UserComment = co.UserComment }, co);
            return co.UserComment.ToString();
        }
         

        [HttpGet("GetComments")]
        public ActionResult<IEnumerable<CommentDto>> GetComments() 
            {
            IEnumerable<Comment> comments = _repository.GetComments();
            comments = comments.OrderBy(e => e.Time);
            IEnumerable<CommentDto> c = comments.Select(e => new CommentDto { UserComment = e.UserComment, Name = e.Name });
            c = c.Reverse().Take(5);
            return Ok(c);
        }
        [HttpPost("Register")]
        public ActionResult<string> Register(User user)
        {
            bool validUserName = true;
            User u = new User { userName = user.userName, password = user.password, address = user.address };
            IEnumerable<User> users = _repository.AllUsers();
            for (int i = 0; i < users.Count(); i++)
            {
                User s = users.ElementAt(i);
                if (s.userName == u.userName)
                    validUserName = false;
            }
            if (validUserName == true)
            {
                _repository.AddUser(user);

                return Ok("User successfully registered.");
            }
            return Ok("Username not available.");
        }
        [Authorize(AuthenticationSchemes = "ChessAPIAuthentication")]
        [HttpGet("GetVersionA")]
        public ActionResult<string> GetVersionA()
        {

            return Ok("1.0.0 (auth)");
        }

        [Authorize(AuthenticationSchemes = "ChessAPIAuthentication")]
        [HttpGet("PurchaseItem/{id}")]
        public ActionResult<Order> PurchaseItem(Int64 id)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string _userName = c.Value;
            Order order = new Order { productId = id, userName = _userName };
            return Ok(order);

        }


        [Authorize(AuthenticationSchemes = "ChessAPIAuthenticationAuthentication")]
        [HttpGet("PairMe")]
        public ActionResult<GameRecord> PairMe()
        {
            IEnumerable<User> users = _repository.AllUsers();
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string _userName = c.Value;

            IEnumerable<User> _p1 = users.Where(e => e.userName == _userName);
            User _player1 = _p1.ElementAt(0);
            IEnumerable<GameRecord> records = _repository.GetRecords();
            bool waiting = true;
            string _gameId = Guid.NewGuid().ToString();
            GameRecord paired = new GameRecord
            {
                gameId = _gameId,
                player1 = _userName,
                state = "wait",
                player2 = null,
                lastMovePlayer1 = null,
                lastMovePlayer2 = null
            };
            foreach (GameRecord record in records)
            {
                if (record.state == "wait" && record.player1 != paired.player1 && record.player2 == null)
                {
                    _repository.RemoveRecord(record);
                    record.player2 = paired.player1;
                    record.state = "progress";
                    waiting = true;
                    paired = record;
                }
            }
            foreach (GameRecord record in records)
            {
                if (record.state == "wait" && record.player1 == paired.player1)
                {
                    paired.gameId = record.gameId;
                    waiting = false;
                }
            }

            if (waiting == true)
                _repository.AddRecord(paired);


            return Ok(new GameRecordOut
            {
                gameId = paired.gameId,
                player1 = paired.player1,
                player2 = paired.player2,
                lastMovePlayer1 = paired.lastMovePlayer1,
                lastMovePlayer2 = paired.lastMovePlayer2,
                state = paired.state
            });



        }
        [Authorize(AuthenticationSchemes = "ChessAPIAuthenticationAuthentication")]
        [HttpGet("TheirMove/{gameId}")]
        public ActionResult<string> TheirMove(string gameId)
        {
            IEnumerable<User> users = _repository.AllUsers();
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string _userName = c.Value;

            IEnumerable<User> _p1 = users.Where(e => e.userName == _userName);
            User _player1 = _p1.ElementAt(0);
            IEnumerable<GameRecord> records = _repository.GetRecords();
            string opponentMove = null;
            bool noOpponent = true;
            bool myId = false;
            bool noSuchGameId = true;
            bool opponentHasMoved = false;
            foreach (GameRecord record in records)
            {
                if (record.gameId == gameId)
                {
                    noSuchGameId = false;
                    if (record.player1 == _userName)
                    {
                        myId = true;
                        if (record.state == "progress")
                        {
                            noOpponent = false;
                            if (record.lastMovePlayer2 != null)
                            {
                                opponentHasMoved = true;
                                opponentMove = record.lastMovePlayer2;
                            }
                        }


                    }
                }
            }
            if (noSuchGameId == true)
                return Ok("no such gameId");
            else if (myId == false)
                return Ok("not your game id");
            else if (noOpponent == true)
                return Ok("You do not have an opponent yet.");
            else if (opponentHasMoved == false)
                return Ok("Your opponent has not moved yet.");
            return Ok(opponentMove);


        }
        [Authorize(AuthenticationSchemes = "ChessAPIAuthenticationAuthentication")]
        [HttpPost("MyMove")]
        public ActionResult<string> MyMove(GameMove myMove)
        {
            IEnumerable<User> users = _repository.AllUsers();
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string _userName = c.Value;

            IEnumerable<User> _p1 = users.Where(e => e.userName == _userName);
            User _player1 = _p1.ElementAt(0);
            IEnumerable<GameRecord> records = _repository.GetRecords();
            bool IdExists = false;
            bool noOpponent = true;
            bool canMove = false;
            GameRecord move = null;
            foreach (GameRecord record in records)
            {
                if (record.gameId == myMove.gameId)
                {
                    IdExists = true;
                    if (record.state == "progress")
                    {
                        noOpponent = false;
                        if (record.player1 == _userName)
                        {
                            if (record.lastMovePlayer1 == null)
                            {
                                canMove = true;
                                record.lastMovePlayer1 = myMove.move;
                                _repository.RemoveRecord(record);
                                move = record;
                            }
                        }
                        else if (record.player2 == _userName)
                        {
                            if (record.lastMovePlayer2 == null)
                            {
                                canMove = true;
                                record.lastMovePlayer2 = myMove.move;
                                _repository.RemoveRecord(record);
                                move = record;

                            }
                        }

                    }
                }
            }
            if (IdExists == false)
                return Ok("no such game id");
            else if (noOpponent == true)
                return Ok("You do not have an opponent yet.");
            else if (canMove == false)
                return Ok("It is not your turn.");
            if (canMove == true)
            {
                _repository.AddRecord(move);
                return Ok("move registered");
            }
            return Ok("Something went wrong.");
        }


        [Authorize(AuthenticationSchemes = "ChessAPIAuthenticationAuthentication")]
        [HttpGet("QuitGame/{gameId}")]

        public ActionResult<string> QuitGame(string gameId)
        {
            IEnumerable<User> users = _repository.AllUsers();
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string _userName = c.Value;

            IEnumerable<User> _p1 = users.Where(e => e.userName == _userName);
            User _player1 = _p1.ElementAt(0);
            IEnumerable<GameRecord> records = _repository.GetRecords();
            bool idExists = false;
            bool userExists = false;
            bool myGameId = false;
            foreach (GameRecord record in records)
            {
                if (record.gameId == gameId)
                {
                    idExists = true;
                    if (record.player1 == _userName || record.player2 == _userName)
                    {
                        myGameId = true;
                        _repository.RemoveRecord(record);
                    }

                }
                if (record.player1 == _userName || record.player2 == _userName)
                    userExists = true;
            }
            if (idExists == false)
                return Ok("no such gameId");
            else if (userExists == false)
                return Ok("You have not started a game.");
            else if (myGameId == false)
                return Ok("not your game id");

            return Ok("game over");
        }
    }

    }

     