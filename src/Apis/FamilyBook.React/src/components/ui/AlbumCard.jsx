import { Card, CardContent, CardDescription, CardHeader, CardTitle } from './Card'
import { getAlbumCoverUrl } from '@/config/gallery'

export const AlbumCard = ({ album, onClick }) => {
  return (
    <Card 
      className="cursor-pointer hover:shadow-lg transition-shadow overflow-hidden"
      onClick={() => onClick(album)}
    >
      <div className="aspect-square overflow-hidden">
        <img
          src={getAlbumCoverUrl(album)}
          alt={album.name}
          className="w-full h-full object-cover hover:scale-105 transition-transform duration-300"
        />
      </div>
      <CardHeader>
        <CardTitle className="text-lg">{album.name}</CardTitle>
        <CardDescription>{album.description}</CardDescription>
        <CardDescription className="text-xs">
          {album.photos.length} {album.photos.length === 1 ? 'foto' : 'foto'}
        </CardDescription>
      </CardHeader>
    </Card>
  )
}
