import { AlbumCard } from './AlbumCard'

export const AlbumGrid = ({ albums, onAlbumClick }) => {
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      {albums.map(album => (
        <AlbumCard 
          key={album.id} 
          album={album} 
          onClick={onAlbumClick}
        />
      ))}
    </div>
  )
}
