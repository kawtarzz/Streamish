import React from "react";
import Video from './Video.js';
import { getAllVideos, searchVideos } from "../modules/videoManager.js";
import { useState, useEffect } from "react";

const VideoList = () => {
  const [videos, setVideos] = useState([]);
  const [searchTerm, setSearchTerm] = useState("");

  // searched terms
  const getVideos = () => {
    getAllVideos().then(videos => setVideos(videos));
  };

  const handleSearch = () => {
    searchVideos(searchTerm).then(videos => setVideos(videos))
  };

  useEffect(() => {
    getVideos();
  }, []);

  return (
    <div className="container">
      <div className="row justify-content-center">
        <input
          type="text"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
        <button onClick={handleSearch}>Search</button>
      </div>
      <div className="row justify-content-center">
        {videos.map((video) => (
          <Video video={video} key={video.id} />
        ))}
      </div>
    </div>
  );
};

export default VideoList;