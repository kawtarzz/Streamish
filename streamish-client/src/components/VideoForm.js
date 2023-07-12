import { addVideo } from "../modules/videoManager";
import { useState } from "react";
import { useNavigate } from "react-router-dom";

export const VideoForm = () => {
  const [title, setTitle] = useState("");
  const [url, setUrl] = useState("");
  const [description, setDescription] = useState("");
  const navigate = useNavigate();

  const handleSubmit = (e) => {
    e.preventDefault();

    //create new video object with user entered data
    // send to database as part of POST 

    const newVideo = {
      title: title,
      url: url,
      description: description,
    };

    addVideo(newVideo).then(() => {
      //reset form fields
      setTitle("");
      setUrl("");
      setDescription("");
      // navigate home
      navigate("/");
    });
  };
  return (
    <div>
      <h3> Add New Video</h3>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Title: </label>
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
          />
        </div>
        <div>
          <label>Url: </label>
          <input
            type="text"
            value={url}
            onChange={(e) => setUrl(e.target.value)}
            required
          />
        </div>
        <div>
          <label>Description: </label>
          <input
            type="text"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            required
          />
        </div>

      </form>
    </div>
  )
}